using Fody;
#pragma warning disable 618

public class TestBase
{
    static TestBase()
    {
        AssemblyLocation.CurrentDirectory = CodeBaseLocation.CurrentDirectory;
    }
}