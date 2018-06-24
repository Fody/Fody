using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace Fody
{
    /// <summary>
    /// Uses <see cref="Assembly.CodeBase"/> to derive the current directory.
    /// </summary>
    [Obsolete(OnlyForTesting.Message)]
    public static class WeaverTestHelper
    {
        public static TestResult ExecuteTestRun(
            this BaseModuleWeaver weaver,
            string assemblyPath,
            bool runPeVerify = true,
            Action<ModuleDefinition> afterExecuteCallback = null,
            Action<ModuleDefinition> beforeExecuteCallback = null,
            string assemblyName = null,
            IEnumerable<string> ignoreCodes = null)
        {
            assemblyPath = Path.Combine(CodeBaseLocation.CurrentDirectory, assemblyPath);
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
            var targetSymbolsPath = Path.ChangeExtension(targetAssemblyPath, "pdb");
            var symbolsPath = Path.ChangeExtension(assemblyPath, "pdb");
            File.Copy(assemblyPath, targetAssemblyPath, true);
            File.Copy(symbolsPath, targetSymbolsPath, true);

            using (var assemblyResolver = new MockAssemblyResolver())
            {
                var typeCache = CacheTypes(weaver, assemblyResolver);

                var testStatus = new TestResult();
                weaver.LogDebug = text => testStatus.AddMessage(text, MessageImportanceDefaults.Debug);
                weaver.LogInfo = text => testStatus.AddMessage(text, MessageImportanceDefaults.Info);
                weaver.LogMessage = (text, messageImportance) => testStatus.AddMessage(text, messageImportance);
                weaver.LogWarning = text => testStatus.AddWarning(text, null);
                weaver.LogWarningPoint = (text, sequencePoint) => testStatus.AddWarning(text, sequencePoint);
                weaver.LogError = text => testStatus.AddError(text, null);
                weaver.LogErrorPoint = (text, sequencePoint) => testStatus.AddError(text, sequencePoint);
                weaver.AssemblyFilePath = targetAssemblyPath;
                weaver.FindType = typeCache.FindType;
                weaver.TryFindType = typeCache.TryFindType;
                weaver.ResolveAssembly = assemblyResolver.Resolve;
                var readerParameters = new ReaderParameters(ReadingMode.Immediate)
                {
                    AssemblyResolver = assemblyResolver,
                    SymbolReaderProvider = new SymbolReaderProvider(),
                    ReadWrite = true,
                    ReadSymbols = true,
                };

                using (var module = ModuleDefinition.ReadModule(targetAssemblyPath, readerParameters))
                {
                    module.Assembly.Name.Name = assemblyName;
                    weaver.ModuleDefinition = module;
                    weaver.TypeSystem = new TypeSystem(typeCache.FindType, module);
                    beforeExecuteCallback?.Invoke(module);

                    weaver.Execute();
                    ReferenceCleaner.CleanReferences(module, weaver, weaver.LogDebug);

                    afterExecuteCallback?.Invoke(module);

                    module.Write();
                }

                if (runPeVerify)
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

                    ignoreList.Add("0x80070002");
                    PeVerifier.ThrowIfDifferent(assemblyPath, targetAssemblyPath, ignoreList, Path.GetDirectoryName(assemblyPath));
                }

                testStatus.Assembly = Assembly.Load(File.ReadAllBytes(targetAssemblyPath));
                testStatus.AssemblyPath = targetAssemblyPath;
                return testStatus;
            }
        }

        static TypeCache CacheTypes(BaseModuleWeaver weaver, MockAssemblyResolver assemblyResolver)
        {
            var typeCache = new TypeCache(assemblyResolver.Resolve);
            typeCache.BuildAssembliesToScan(weaver);
            return typeCache;
        }
    }
}