using System.IO;
using ApprovalTests;
using Fody;
using Xunit;

public class ProjectWeaversReaderTests : TestBase
{
    [Fact]
    public void Invalid()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory;
        var path = Path.Combine(currentDirectory, @"Fody\ProjectWeaversReaderTests\Invalid.txt");

        var exception = Assert.Throws<WeavingException>(() => XDocumentEx.Load(path));
        Approvals.Verify(exception.Message.Replace(currentDirectory, ""));
    }
}