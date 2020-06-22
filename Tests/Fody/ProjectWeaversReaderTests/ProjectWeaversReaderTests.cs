using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class ProjectWeaversReaderTests
{
    [Fact]
    public Task Invalid()
    {
        var path = @"Fody\ProjectWeaversReaderTests\Invalid.txt";

        var exception = Assert.ThrowsAny<Exception>(() => XDocumentEx.Load(path));
        return VerifyXunit.Verifier.Verify(exception!.Message);
    }
}