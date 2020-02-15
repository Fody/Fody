namespace WithNoPdb
{
    public class Class1
    {
        [SampleWeaver.SymbolValidation(HasSymbols = false)]
        public void Method()
        {

        }
    }

}