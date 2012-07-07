public static class ObjectTypeName
{
    public static string GetTypeName(this object o)
    {
        var type = o.GetType();
        return string.Format("{1}, {0}", type.Assembly.GetName().Name, type.FullName);
    }
    public static string GetAssemblyName(this object o)
    {
        var type = o.GetType();
        return type.Assembly.GetName().Name;
    }

}