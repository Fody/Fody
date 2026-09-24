using System.Threading.Tasks;

public class ProjectWeaversReaderTests
{
    [Test]
    public async Task Invalid()
    {
        var path = @"Fody\ProjectWeaversReaderTests\Invalid.txt";

        var exception = await Assert.That(() => XDocumentEx.Load(path)).ThrowsException();
        await VerifyTUnit.Verifier.Verify(exception!.Message);
    }
}