using System.IO;
using System.Linq;
using System.Reflection;
using Fody;

public partial class InnerWeaver
{
    public StrongNameKeyPair StrongNameKeyPair;

    public virtual void FindStrongNameKey()
    {
        if (!SignAssembly)
        {
            return;
        }
        var keyFilePath = GetKeyFilePath();
        if (keyFilePath != null)
        {
            if (!File.Exists(keyFilePath))
            {
                throw new WeavingException($"KeyFilePath was defined but file does not exist. '{keyFilePath}'.");
            }
            StrongNameKeyPair = new StrongNameKeyPair(File.OpenRead(keyFilePath));
        }
    }

    string GetKeyFilePath()
    {
        if (KeyFilePath != null)
        {
            KeyFilePath = Path.GetFullPath(KeyFilePath);
            Logger.LogDebug($"Using strong name key from KeyFilePath '{KeyFilePath}'.");
            return KeyFilePath;
        }

        var assemblyKeyFileAttribute = ModuleDefinition
            .Assembly
            .CustomAttributes
            .FirstOrDefault(x => x.AttributeType.Name == "AssemblyKeyFileAttribute");
        if (assemblyKeyFileAttribute != null)
        {
            var keyFileSuffix = (string) assemblyKeyFileAttribute.ConstructorArguments.First().Value;
            var keyFilePath = Path.Combine(IntermediateDirectoryPath, keyFileSuffix);
            Logger.LogDebug($"Using strong name key from [AssemblyKeyFileAttribute(\"{keyFileSuffix}\")] '{keyFilePath}'");
            return keyFilePath;
        }
        Logger.LogDebug("No strong name key found");
        return null;
    }
}