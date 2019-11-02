using ApprovalTests.Reporters;

[assembly: UseReporter(typeof(DiffReporter),typeof(AllFailingTestsClipboardReporter))]