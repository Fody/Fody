using ApprovalTests;
using Fody;
using Xunit;
using Xunit.Abstractions;

public class ProjectWeaversReaderTests :
    XunitApprovalBase
{
    [Fact]
    public void Invalid()
    {
        var path =  @"Fody\ProjectWeaversReaderTests\Invalid.txt";

        var exception = Assert.Throws<WeavingException>(() => XDocumentEx.Load(path));
        Approvals.Verify(exception.Message);
    }

    public ProjectWeaversReaderTests(ITestOutputHelper output) : 
        base(output)
    {
    }
}