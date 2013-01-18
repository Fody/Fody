using System.IO;
using System.Linq;
using System.Reflection;

public partial class InnerWeaver 
{
    public StrongNameKeyPair StrongNameKeyPair;

    public void FindStrongNameKey()
    {
        var keyFilePath = GetKeyFilePath();
        if (keyFilePath != null)
        {
            if (!File.Exists(keyFilePath))
            {
                throw new WeavingException(string.Format("KeyFilePath was defined but file does not exist. '{0}'.", keyFilePath));
            }
            StrongNameKeyPair = new StrongNameKeyPair(File.OpenRead(keyFilePath));
        }
    }


    string GetKeyFilePath()
   {
       if (KeyFilePath != null)
       {
           KeyFilePath = Path.GetFullPath(KeyFilePath);
           Logger.LogInfo(string.Format("Using strong name key from KeyFilePath '{0}'.", KeyFilePath));
           return KeyFilePath;
       }
       //public AssemblyKeyFileAttribute(string keyFile)
       var assemblyKeyFileAttribute = ModuleDefinition
           .Assembly
           .CustomAttributes
           .FirstOrDefault(x => x.AttributeType.Name == "AssemblyKeyFileAttribute");
       if (assemblyKeyFileAttribute != null)
       {
           var keyFileSuffix = (string) assemblyKeyFileAttribute.ConstructorArguments.First().Value;
           var ketFilePath = Path.Combine(IntermediateDirectoryPath, keyFileSuffix);
           Logger.LogInfo(string.Format("Using strong name key from [AssemblyKeyFileAttribute(\"{0}\")] '{1}'", keyFileSuffix, ketFilePath));
           return ketFilePath ;
       }
       Logger.LogInfo("No strong name key found");
       return null;
   }




}