using ApprovalTests;
using Fody;
using Xunit;

public class ProjectWeaversReaderTests : 
    TestBase
{
    [Fact]
    public void Invalid()
    {
        var path =  @"Fody\ProjectWeaversReaderTests\Invalid.txt";

        var exception = Assert.Throws<WeavingException>(() => XDocumentEx.Load(path));
        Approvals.Verify(exception.Message.TrimStart('\\'));
    }
}