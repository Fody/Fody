using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Fody
{
    /// <summary>
    /// Uses <see cref="Assembly.CodeBase"/> to derive the current directory.
    /// Only for test usage. Only for development purposes when building Fody addins. The API may change in minor releases.
    /// </summary>
    public static class WeaverTestHelper
    {
        public static TestResult ExecuteTestRun(
            this BaseModuleWeaver weaver,
            string assemblyPath,
            bool runPeVerify = true,
            Action<ModuleDefinition>? afterExecuteCallback = null,
            Action<ModuleDefinition>? beforeExecuteCallback = null,
            string? assemblyName = null,
            IEnumerable<string>? ignoreCodes = null,
            bool writeSymbols = false)
        {
            assemblyPath = Path.GetFullPath(assemblyPath);
            Guard.FileExists(nameof(assemblyPath), assemblyPath);
            var fodyTempDir = Path.Combine(Path.GetDirectoryName(assemblyPath), "fodytemp");
            Directory.CreateDirectory(fodyTempDir);

            string targetFileName;
            if (assemblyName == null)
            {
                assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
                targetFileName = Path.GetFileName(assemblyPath);
            }
            else
            {
                targetFileName = assemblyName + ".dll";
            }

            var targetAssemblyPath = Path.Combine(fodyTempDir, targetFileName);
            File.Delete(targetAssemblyPath);

            using var assemblyResolver = new TestAssemblyResolver();
            var typeCache = CacheTypes(weaver, assemblyResolver);

            var testStatus = new TestResult();
            weaver.LogDebug = text => testStatus.AddMessage(text, MessageImportanceDefaults.Debug);
            weaver.LogInfo = text => testStatus.AddMessage(text, MessageImportanceDefaults.Info);
            weaver.LogMessage = (text, messageImportance) => testStatus.AddMessage(text, messageImportance);
            weaver.LogWarning = text => testStatus.AddWarning(text, null);
            weaver.LogWarningPoint = (text, sequencePoint) => testStatus.AddWarning(text, sequencePoint);
            weaver.LogError = text => testStatus.AddError(text, null);
            weaver.LogErrorPoint = (text, sequencePoint) => testStatus.AddError(text, sequencePoint);
            weaver.AssemblyFilePath = assemblyPath;
            weaver.FindType = typeCache.FindType;
            weaver.TryFindType = typeCache.TryFindType;
            weaver.ResolveAssembly = assemblyResolver.Resolve;
            var readerParameters = new ReaderParameters
            {
                AssemblyResolver = assemblyResolver,
                SymbolReaderProvider = new SymbolReaderProvider(),
                ReadWrite = false,
                ReadSymbols = true,
            };

            using (var module = ModuleDefinition.ReadModule(assemblyPath, readerParameters))
            {
                module.Assembly.Name.Name = assemblyName;
                weaver.ModuleDefinition = module;
                weaver.TypeSystem = new TypeSystem(typeCache.FindType, module);
                beforeExecuteCallback?.Invoke(module);

                weaver.Execute();
                ReferenceCleaner.CleanReferences(module, weaver, weaver.LogDebug);

                afterExecuteCallback?.Invoke(module);

                var writerParameters = new WriterParameters
                {
                    WriteSymbols = writeSymbols
                };
                if (writeSymbols)
                {
                    writerParameters.SymbolWriterProvider = new EmbeddedPortablePdbWriterProvider();
                }

                module.Write(targetAssemblyPath, writerParameters);
            }

            if (runPeVerify && IsWindows())
            {
                List<string> ignoreList;
                if (ignoreCodes == null)
                {
                    ignoreList = new List<string>();
                }
                else
                {
                    ignoreList = ignoreCodes.ToList();
                }

                PeVerifier.ThrowIfDifferent(assemblyPath, targetAssemblyPath, ignoreList, Path.GetDirectoryName(assemblyPath));
            }

            testStatus.Assembly = Assembly.Load(File.ReadAllBytes(targetAssemblyPath));
            testStatus.AssemblyPath = targetAssemblyPath;
            return testStatus;
        }

        static bool IsWindows()
        {
            var platform = Environment.OSVersion.Platform.ToString();
            return platform.StartsWith("win", StringComparison.OrdinalIgnoreCase);
        }

        static TypeCache CacheTypes(BaseModuleWeaver weaver, TestAssemblyResolver assemblyResolver)
        {
            var typeCache = new TypeCache(assemblyResolver.Resolve);
            typeCache.BuildAssembliesToScan(weaver);
            return typeCache;
        }
    }
}