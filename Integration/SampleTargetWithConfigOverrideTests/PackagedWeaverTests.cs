using System;
using System.Globalization;
using System.IO;
using JetBrains.Annotations;
using Xunit;

[assembly: SampleWeaver.Sample]

namespace SampleTarget
{
    public class PackagedWeaverTests
    {
        [Fact]
        public void SampleWeaverAddedExtraFileDuringBuild()
        {
            var assemblyPath = GetAssemblyLocation();
            var targetFolder = Path.GetDirectoryName(assemblyPath);
            var extraFilePath = Path.Combine(targetFolder, "SomeExtraFile.txt");
            var extraFileContent = File.ReadAllText(extraFilePath);
            var assemblyBuildTime = File.GetLastWriteTime(assemblyPath);

            Assert.True(DateTime.TryParse(extraFileContent, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var weaverExecutionTime));
            var elapsed = assemblyBuildTime - weaverExecutionTime;
            Assert.True(elapsed < TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void SampleWeaverRemovedObsoleteDependenciesDuringBuild()
        {
            var assemblyPath = GetAssemblyLocation();
            var targetFolder = Path.GetDirectoryName(assemblyPath);

            var sampleWeaverFiles = Directory.EnumerateFiles(targetFolder, "SampleWeaver.*");

            Assert.Empty(sampleWeaverFiles);
        }

        [Fact]
        public void NullGuardsAreActive()
        {
            Assert.Throws<ArgumentNullException>(() => GuardedMethod(null));
        }

        [Fact]
        public void WeaverConfigurationIsReadFromProperty()
        {
            var type = Type.GetType("SampleWeaverTest.Configuration");
            var content = (string)type.GetField("Content").GetValue(null);
            var expectedContent = "<SampleWeaver MyProperty=\"CustomPropertyValue\">\r\n  <Content>Override</Content>\r\n</SampleWeaver>".Replace("\r\n", Environment.NewLine);

            Assert.Equal(expectedContent, content);

            var propertyValue = (string)type.GetField("PropertyValue").GetValue(null);
            const string expectedPropertyValue = "CustomPropertyValue";

            Assert.Equal(expectedPropertyValue, propertyValue);
        }

        [NotNull]
        public object GuardedMethod([NotNull] object parameter)
        {
            return parameter;
        }

        string GetAssemblyLocation()
        {
#pragma warning disable IDE0022 // Use expression body for methods
#if NETFRAMEWORK
            return new Uri(GetType().Assembly.CodeBase).LocalPath;
#else
            return GetType().Assembly.Location;
#endif
        }
    }
}
