using System;
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
        public static TestResult ExecuteTestRun(this BaseModuleWeaver weaver, string assemblyPath)
        {
            var fodyTempDir = Path.Combine(Path.GetDirectoryName(assemblyPath), "fodytemp");
            Directory.CreateDirectory(fodyTempDir);
            var targetAssemblyPath = Path.Combine(fodyTempDir, Path.GetFileName(assemblyPath));
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

                weaver.FindType = typeCache.FindType;
                weaver.ResolveAssembly = assemblyResolver.Resolve;

                var readerParameters = new ReaderParameters(ReadingMode.Immediate)
                {
                    AssemblyResolver = assemblyResolver,
                    SymbolReaderProvider = new SymbolReaderProvider(),
                    ReadWrite = true,
                    ReadSymbols = true
                };

                using (var moduleDefinition = ModuleDefinition.ReadModule(targetAssemblyPath, readerParameters))
                {
                    weaver.ModuleDefinition = moduleDefinition;

                    weaver.Execute();

                    moduleDefinition.Write();
                }

                PeVerifier.ThrowIfDifferent(assemblyPath, targetAssemblyPath, new[] {"0x80070002"});
                testStatus.Assembly = Assembly.Load(File.ReadAllBytes(targetAssemblyPath));
                return testStatus;
            }
        }

        static TypeCache CacheTypes(BaseModuleWeaver weaver, MockAssemblyResolver assemblyResolver)
        {
            var definitions = weaver.GetAssembliesForScanning()
                .Select(assemblyResolver.Resolve)
                .Where(definition => definition != null);

            var typeCache = new TypeCache();
            typeCache.Initialise(definitions);
            return typeCache;
        }
    }
}