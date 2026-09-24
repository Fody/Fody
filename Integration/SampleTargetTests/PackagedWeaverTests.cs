#pragma warning disable IDE0022 // Use expression body for methods

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
            var assemblyPath =  GetAssemblyLocation();
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
            var assemblyPath =  GetAssemblyLocation();
            var targetFolder = Path.GetDirectoryName(assemblyPath);

            var sampleWeaverFiles = Directory.EnumerateFiles(targetFolder, "SampleWeaver.*");

            await Assert.That(sampleWeaverFiles).IsEmpty();
        }

        [Test]
        public async Task SampleWeaverRemovedWeaverFromDepsJsonDuringBuild()
        {
            var assemblyPath =  GetAssemblyLocation();
            var depsJson = Path.ChangeExtension(assemblyPath, "deps.json");

            if (!File.Exists(depsJson))
                return;

            var content = File.ReadAllText(depsJson);

            await Assert.That(content).DoesNotContain("\"lib/netstandard2.0/SampleWeaver.dll\":");
        }

        [Test]
        public async Task NullGuardsAreActive()
        {
            await Assert.That(() => GuardedMethod(null)).Throws<ArgumentNullException>();
        }

        [Test]
        public async Task WeaverConfigurationIsRead()
        {
            var type = Type.GetType("SampleWeaverTest.Configuration");
            var content = (string)type.GetField("Content").GetValue(null);
            var expectedContent = "<SampleWeaver MyProperty=\"PropertyValue\">\r\n  <Content>Test</Content>\r\n</SampleWeaver>".Replace("\r\n", Environment.NewLine);

            await Assert.That(content).IsEqualTo(expectedContent);

            var propertyValue = (string)type.GetField("PropertyValue").GetValue(null);
            const string expectedPropertyValue = "PropertyValue";

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
#if NETFRAMEWORK
            return new Uri(GetType().Assembly.CodeBase).LocalPath;
#else
            return GetType().Assembly.Location;
#endif
        }

    }
}
