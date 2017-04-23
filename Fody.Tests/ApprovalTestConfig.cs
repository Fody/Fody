using ApprovalTests.Reporters;
#if(DEBUG)
[assembly: UseReporter(typeof(DiffReporter),typeof(AllFailingTestsClipboardReporter))]
#endif