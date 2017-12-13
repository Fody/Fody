using Moq;
using Xunit;

public class WeaverAssemblyPathFinderTests : TestBase
{
    [Fact(Skip = "todo")]
    //TODO:
    public void Valid()
    {
        var finder = new Processor
        {
            ContainsTypeChecker = new Mock<ContainsTypeChecker>().Object,
        };
        finder.FindAssemblyPath("Name");
    }
}