using Moq;
using NUnit.Framework;

[TestFixture]
public class WeaverAssemblyPathFinderTests
{
    [Test]
    [Explicit]
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