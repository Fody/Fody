using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ProjectWeaversReaderTests :
    VerifyBase
{
    [Fact]
    public Task Invalid()
    {
        var path =  @"Fody\ProjectWeaversReaderTests\Invalid.txt";

        var exception = Assert.ThrowsAny<Exception>(() => XDocumentEx.Load(path));
        return Verify(exception.Message);
    }

    public ProjectWeaversReaderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}