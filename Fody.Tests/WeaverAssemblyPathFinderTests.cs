using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class WeaverAssemblyPathFinderTests
{
    [Test]
    //TODO:
    public void Valid()
    {
        var finder = new WeaverAssemblyPathFinder
                         {
                             ContainsTypeChecker = Substitute.For<ContainsTypeChecker>(),
                             AddinFilesEnumerator = Substitute.For<AddinFilesEnumerator>(),
                         };
        finder.FindAssemblyPath("Name");
    }
}