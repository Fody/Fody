#if NET46 // TODO: Remove when ApprovalTests supports .NET Core
using ApprovalTests.Reporters;
[assembly: UseReporter(typeof(DiffReporter),typeof(AllFailingTestsClipboardReporter))]
#endif