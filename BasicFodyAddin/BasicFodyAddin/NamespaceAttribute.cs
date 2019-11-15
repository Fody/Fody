using System;

/// <summary>
/// Namespace to use.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class NamespaceAttribute : Attribute
{
    /// <summary>
    /// Initialize a new instance of <see cref="NamespaceAttribute"/>
    /// </summary>
    public NamespaceAttribute(string @namespace)
    {
    }
}