using System;

namespace SampleWeaver
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SymbolValidationAttribute : Attribute
    {
        public SymbolValidationAttribute()
        {
        }

        public bool HasSymbols { get; set; }
    }
}
