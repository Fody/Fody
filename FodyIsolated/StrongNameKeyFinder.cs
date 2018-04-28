using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Fody;

public partial class InnerWeaver
{
    public StrongNameKeyPair StrongNameKeyPair;
    public byte[] KeyFileBytes = null;

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

            var keyFileBytes = File.ReadAllBytes(keyFilePath);
            StrongNameKeyPair = new StrongNameKeyPair(KeyFileBytes);

            try
            {
                // Ensure that we can generate the public key from the key file. This requires the private to
                // work. If we cannot generate the public key, an ArgumentException will be thrown. In this case,
                // the assembly is delay-signed with a public only keyfile.
                var bytes = StrongNameKeyPair.PublicKey;
            }
            catch(ArgumentException)
            {
                // We know that we cannot sign the assembly with this keyfile. Let's assume that it is a public
                // only keyfile and pass along all the bytes. Mono.Cecil will parse the file to create the public
                // key and assign that to the assembly name without signing it.
                StrongNameKeyPair = null;
                KeyFileBytes = keyFileBytes;
            }
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