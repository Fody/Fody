public partial class InnerWeaver
{
    public byte[]? StrongNameKeyBlob;
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
                if (IsPrivateKeyFile(fileBytes))
                {
                    StrongNameKeyBlob = fileBytes;
                    // Cecil will update the assembly name's public key.
                    return;
                }
                Logger.LogWarning("Key file is not private key, fall back to delay-signing.");
            }

            // Fall back to delay signing, this was the original behavior, however that does not work in NETSTANDARD (s.a.)
            Logger.LogDebug("Prepare public key for delay-signing.");

            // We know that we cannot sign the assembly with this key-file. Let's assume that it is a public
            // only key-file and pass along all the bytes.
            StrongNameKeyBlob = null;
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

    static bool IsPrivateKeyFile(byte[] blob) => blob.Length >= 12
            && blob[0] == 0x07 // PRIVATEKEYBLOB (0x07)
            && blob[1] == 0x02 // Version (0x02)
            && blob[2] == 0x00 // Reserved (word)
            && blob[3] == 0x00
            && BitConverter.ToUInt32(blob, 8) == 0x32415352; // DWORD magic = RSA2
}