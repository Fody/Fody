using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

public class StrongNameKeyFinder 
{
    public InnerWeaver InnerWeaver;
    public ILogger Logger;
    public StrongNameKeyPair StrongNameKeyPair;
    public ModuleDefinition ModuleDefinition;

    public void Execute()
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
       if (InnerWeaver.KeyFilePath != null)
       {
           Logger.LogInfo(string.Format("Using strong name key from KeyFilePath '{0}'.", InnerWeaver.KeyFilePath));
           return InnerWeaver.KeyFilePath;
       }
       //public AssemblyKeyFileAttribute(string keyFile)
       var assemblyKeyFileAttribute = ModuleDefinition
           .Assembly
           .CustomAttributes
           .FirstOrDefault(x => x.AttributeType.Name == "AssemblyKeyFileAttribute");
       if (assemblyKeyFileAttribute != null)
       {
           var keyFileSuffix = (string) assemblyKeyFileAttribute.ConstructorArguments.First().Value;
           var ketFilePath = Path.Combine(InnerWeaver.IntermediateDir, keyFileSuffix);
           Logger.LogInfo(string.Format("Using strong name key from [AssemblyKeyFileAttribute(\"{0}\")] '{1}'", keyFileSuffix, ketFilePath));
           return ketFilePath ;
       }
       Logger.LogInfo("No strong name key found");
       return null;
   }




}