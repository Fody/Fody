namespace DummyAssembly
{
    public class Class1
    {
        int field;

        public void Method(int param)
        {
            field = param;
            param += 1;
            if (param == field)
            {
                field -= 1;
            }
        }
    }
}