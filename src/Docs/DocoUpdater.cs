using Xunit.Abstractions;

public class DocoUpdater :
    XunitApprovalBase
{
    public DocoUpdater(ITestOutputHelper output) :
        base(output)
    {
    }
}