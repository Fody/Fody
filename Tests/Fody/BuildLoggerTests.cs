public class BuildLoggerTests
{
    MockBuildEngine buildEngine = new();
    BuildLogger buildLogger;

    public BuildLoggerTests() =>
        buildLogger = new()
        {
            BuildEngine = buildEngine
        };

    [Test]
    public async Task TreatWarningsAsErrors()
    {
        buildLogger.TreatWarningsAsErrors = true;
        buildLogger.LogWarning("Message", "Code");

        await Assert.That(buildEngine.Errors).HasSingleItem();
        await Assert.That(buildEngine.Warnings).IsEmpty();
    }

    [Test]
    public async Task DontTreatWarningsAsErrors()
    {
        buildLogger.TreatWarningsAsErrors = false;
        buildLogger.LogWarning("Message", "Code");

        await Assert.That(buildEngine.Warnings).HasSingleItem();
        await Assert.That(buildEngine.Errors).IsEmpty();
    }

    private class MockBuildEngine : IBuildEngine
    {
        public List<BuildWarningEventArgs> Warnings = new();
        public List<BuildErrorEventArgs> Errors = new();

        public void LogErrorEvent(BuildErrorEventArgs e) =>
            Errors.Add(e);

        public void LogWarningEvent(BuildWarningEventArgs e) =>
            Warnings.Add(e);

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
        }

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
        }

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs) =>
            false;

        public bool ContinueOnError => false;
        public int LineNumberOfTaskNode => 0;
        public int ColumnNumberOfTaskNode => 0;
        public string ProjectFileOfTaskNode => "";
    }
}
