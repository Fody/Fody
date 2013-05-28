using System;
using System.IO;
using System.Reflection;

public partial class InnerWeaver
{
    ResolveEventHandler CreateAssemblyResolveHandler(string assemblyPath)
    {
        var basePath = Path.GetDirectoryName(assemblyPath);

        return (obj, args) =>
        {
            try
            {
                var assemblyFullName = new AssemblyName(args.Name);
                var assemblyShortName = assemblyFullName.Name;
                var location = Path.Combine(basePath, assemblyShortName);

                if (File.Exists(location + ".dll"))
                {
                    return Assembly.LoadFile(location + ".dll");
                }

                if (File.Exists(location + ".exe"))
                {
                    return Assembly.LoadFile(location + ".exe");
                }

                return null;
            }
            catch
            {
                return null;
            }
        };
    }
}

