using Fody;

public class TestBase
{
    static TestBase()
    {
        AssemblyLocation.CurrentDirectory = CodeBaseLocation.CurrentDirectory;
    }
}