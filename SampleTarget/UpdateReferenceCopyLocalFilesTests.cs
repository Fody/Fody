using System;
using System.Globalization;
using System.IO;

using Xunit;

[assembly: SampleWeaver.Sample]

namespace SampleTarget
{
    public class UpdateReferenceCopyLocalFilesTests
    {
#if IS_INLINE_INTEGRATION_TEST_ACTIVE
        private const string skipReason = null;
#else
        private const string skipReason = "Inline integration tests skipped, since environment variable MSBUILDDISABLENODERESUSE is not set to 1";
#endif

        [Fact(Skip = skipReason)]
        public void SampleWeaverAddedExtraFileDuringBuild()
        {
            var assemblyPath = new Uri(GetType().Assembly.CodeBase).LocalPath;
            var targetFolder = Path.GetDirectoryName(assemblyPath);
            var extraFilePath = Path.Combine(targetFolder, "SomeExtraFile.txt");
            var extraFileContent = File.ReadAllText(extraFilePath);
            var assemblyBuildTime = File.GetLastWriteTime(assemblyPath);

            Assert.True(DateTime.TryParse(extraFileContent, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var weaverExecutionTime));
            var elapsed = assemblyBuildTime - weaverExecutionTime;
            Assert.True(elapsed < TimeSpan.FromMinutes(1));
        }

        [Fact(Skip = skipReason)]
        public void SampleWeaverRemovedObsoleteDependenciesDuringBuild()
        {
            var assemblyPath = new Uri(GetType().Assembly.CodeBase).LocalPath;
            var targetFolder = Path.GetDirectoryName(assemblyPath);

            var sampleWeaverFiles = Directory.EnumerateFiles(targetFolder, "SampleWeaver.*");

            Assert.Empty(sampleWeaverFiles);
        }
    }
}
