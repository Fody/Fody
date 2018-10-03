using ApprovalTests.Reporters;
using Xunit;

[assembly: UseReporter(typeof(ClipboardReporter), typeof(DiffReporter))]
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]