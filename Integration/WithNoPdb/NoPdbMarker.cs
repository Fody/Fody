using WithNoPdb;

[assembly: NoSymbolsMarker]

namespace WithNoPdb
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly)]
    public class NoSymbolsMarkerAttribute : Attribute
    {
    }
}
