#if IS_INLINE_INTEGRATION_TEST_ACTIVE
//  only test if environment variable MSBUILDDISABLENODERESUSE is set to 1";
using System;
using System.Globalization;
using System.IO;
using Xunit;

[assembly: SampleWeaver.Sample]

namespace SampleTarget
{
    using JetBrains.Annotations;

    public class WeaverIntegrationTests
    {
        [Fact]
        public void SampleWeaverAddedExtraFileDuringBuild()
        {
            var assemblyPath = new Uri(GetType().Assembly.CodeBase).LocalPath;
            var targetFolder = AppDomain.CurrentDomain.BaseDirectory;
            var extraFilePath = Path.Combine(targetFolder, "SomeExtraFile.txt");
            var extraFileContent = File.ReadAllText(extraFilePath);
            var assemblyBuildTime = File.GetLastWriteTime(assemblyPath);

            Assert.True(DateTime.TryParse(extraFileContent, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var weaverExecutionTime));
            var elapsed = assemblyBuildTime - weaverExecutionTime;
            Assert.True(elapsed < TimeSpan.FromMinutes(1));
        }

        //[Fact]
        //public void SampleWeaverRemovedObsoleteDependenciesDuringBuild()
        //{
        //    var targetFolder = AppDomain.CurrentDomain.BaseDirectory;

        //    var sampleWeaverFiles = Directory.EnumerateFiles(targetFolder, "SampleWeaver.*");

        //    Assert.Empty(sampleWeaverFiles);
        //}

        [Fact]
        public void NullGuardsAreActive()
        {
            Assert.Throws<ArgumentNullException>(() => GuardedMethod(null));
        }

        [Fact]
        public void WeaverConfigurationIsRead()
        {
            var type = Type.GetType("SampleWeaverTest.Configuration");
            var content = (string)type.GetField("Content").GetValue(null);
            const string expectedContent = "<SampleWeaver MyProperty=\"PropertyValue\">\r\n  <Content>Test</Content>\r\n</SampleWeaver>";

            Assert.Equal(expectedContent, content);

            var propertyValue = (string)type.GetField("PropertyValue").GetValue(null);
            const string expectedPropertyValue = "PropertyValue";

            Assert.Equal(expectedPropertyValue, propertyValue);
        }

        [NotNull]
        public object GuardedMethod([NotNull] object parameter)
        {
            return parameter;
        }
    }
}

#endif