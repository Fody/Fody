using System.Diagnostics;
using NUnit.Framework;

[TestFixture]
public class ContentsFinderTests
{
    [Test]
    public void Simple()
    {
        var contentsFinder = new ContentsFinder();
        Debug.WriteLine(contentsFinder.ContentFilesPath);
    }
}