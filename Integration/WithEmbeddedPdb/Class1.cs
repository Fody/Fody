namespace WithEmbeddedPdb
{
    public class Class1
    {
        [SampleWeaver.SymbolValidation(HasSymbols = true)]
        public void Method()
        {
        }
    }
}