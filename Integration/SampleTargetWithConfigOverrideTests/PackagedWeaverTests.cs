using System;
using System.Globalization;
using System.IO;
using JetBrains.Annotations;

[assembly: SampleWeaver.Sample]

namespace SampleTarget
{
    public class PackagedWeaverTests
    {
        [Test]
        public async Task SampleWeaverAddedExtraFileDuringBuild()
        {
            var assemblyPath = GetAssemblyLocation();
            var targetFolder = Path.GetDirectoryName(assemblyPath);
            var extraFilePath = Path.Combine(targetFolder, "SomeExtraFile.txt");
            var extraFileContent = File.ReadAllText(extraFilePath);
            var assemblyBuildTime = File.GetLastWriteTime(assemblyPath);

            await Assert.That(DateTime.TryParse(extraFileContent, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var weaverExecutionTime)).IsTrue();
            var elapsed = assemblyBuildTime - weaverExecutionTime;
            await Assert.That(elapsed < TimeSpan.FromMinutes(1)).IsTrue();
        }

        [Test]
        public async Task SampleWeaverRemovedObsoleteDependenciesDuringBuild()
        {
            var assemblyPath = GetAssemblyLocation();
            var targetFolder = Path.GetDirectoryName(assemblyPath);

            var sampleWeaverFiles = Directory.EnumerateFiles(targetFolder, "SampleWeaver.*");

            await Assert.That(sampleWeaverFiles).IsEmpty();
        }

        [Test]
        public async Task NullGuardsAreActive()
        {
            await Assert.That(() => GuardedMethod(null)).Throws<ArgumentNullException>();
        }

        [Test]
        public async Task WeaverConfigurationIsReadFromProperty()
        {
            var type = Type.GetType("SampleWeaverTest.Configuration");
            var content = (string)type.GetField("Content").GetValue(null);
            var expectedContent = "<SampleWeaver MyProperty=\"CustomPropertyValue\">\r\n  <Content>Override</Content>\r\n</SampleWeaver>".Replace("\r\n", Environment.NewLine);

            await Assert.That(content).IsEqualTo(expectedContent);

            var propertyValue = (string)type.GetField("PropertyValue").GetValue(null);
            const string expectedPropertyValue = "CustomPropertyValue";

            await Assert.That(propertyValue).IsEqualTo(expectedPropertyValue);
        }

        // public so that NullGuard weaves it
#pragma warning disable TUnit0014
        [NotNull]
        public object GuardedMethod([NotNull] object parameter)
        {
            return parameter;
        }
#pragma warning restore TUnit0014

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
