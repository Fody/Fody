#if (NETSTANDARD)
using StrongNameKeyPair = Mono.Cecil.StrongNameKeyPair;
#else
using StrongNameKeyPair = System.Reflection.StrongNameKeyPair;
#endif

public partial class InnerWeaver
{
    public StrongNameKeyPair? StrongNameKeyPair;
    public byte[]? PublicKey;

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

            var fileBytes = File.ReadAllBytes(keyFilePath);

            if (!DelaySign)
            {
                try
                {
                    Logger.LogDebug("Extract public key from key file for signing.");

                    StrongNameKeyPair = new(fileBytes);
                    // Ensure that we can generate the public key from the key file. This requires the private key to
                    // work. If we cannot generate the public key, an ArgumentException will be thrown. In this case,
                    // the assembly is delay-signed with a public only key-file.
                    // Note: The NETSTANDARD implementation of StrongNameKeyPair.PublicKey does never throw here.
                    PublicKey = StrongNameKeyPair.PublicKey;
                    return;
                }
                catch (ArgumentException)
                {
                    Logger.LogWarning("Failed to extract public key from key file, fall back to delay-signing.");
                }
            }

            // Fall back to delay signing, this was the original behavior, however that does not work in NETSTANDARD (s.a.)
            Logger.LogDebug("Prepare public key for delay-signing.");

            // We know that we cannot sign the assembly with this key-file. Let's assume that it is a public
            // only key-file and pass along all the bytes.
            StrongNameKeyPair = null;
            PublicKey = fileBytes;
        }
    }

    string? GetKeyFilePath()
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
            .FirstOrDefault(_ => _.AttributeType.Name == "AssemblyKeyFileAttribute");
        if (assemblyKeyFileAttribute != null)
        {
            var keyFileSuffix = (string)assemblyKeyFileAttribute.ConstructorArguments.First().Value;
            var keyFilePath = Path.Combine(IntermediateDirectoryPath, keyFileSuffix);
            Logger.LogDebug($"Using strong name key from [AssemblyKeyFileAttribute(\"{keyFileSuffix}\")] '{keyFilePath}'");
            return keyFilePath;
        }
        Logger.LogDebug("No strong name key found");
        return null;
    }
}