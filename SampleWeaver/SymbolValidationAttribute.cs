using System;

namespace SampleWeaver;

[AttributeUsage(AttributeTargets.Method)]
public class SymbolValidationAttribute : Attribute
{
    public bool HasSymbols { get; set; }
}