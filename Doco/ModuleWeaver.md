
## ModuleWeaver Class 

 * The class should be public, instance and not abstract.
 * Have an empty constructor. 
 * A 'ModuleDefinition' property that will be populated during build. It will contain the Cecil representation of the assembly being built
 * A 'Execute' method with the signature that is called as part of a build after the properties have been set.

For example the minimum class would look like this


    public class ModuleWeaver
    {
        public ModuleDefinition ModuleDefinition { get; set; }

        public void Execute()
        {
            ModuleDefinition.Types.Add(new TypeDefinition("MyNamespace", "MyType", TypeAttributes.Public, ModuleDefinition.Import(typeof(object))));
        }
    }


There are also a number of optional properties that will be populated before 'Execute' is called.

 * A 'Config' property that will contain the full element xml from FodyWeavers.xml.
 * A 'LogInfo' and a 'LogWarning' delegate for logging informational and warning messages respectively. 
 * An 'AssemblyResolver' property that will contain a Mono.Cecil.IAssemblyResolver for resolving dependencies.

For example


    public class ModuleWeaver
    {
        public XElement Config { get; set; }

        public Action<string> LogInfo { get; set; }

        public Action<string> LogWarning { get; set; }

        public IAssemblyResolver AssemblyResolver { get; set; }

        public ModuleDefinition ModuleDefinition { get; set; }

        // Init logging delegates to make testing easier
        public ModuleWeaver()
        {
            LogWarning = s => { };
            LogInfo = s => { };
        } 

        public void Execute()
        {
            ModuleDefinition.Types.Add(new TypeDefinition("MyNamespace", "MyType", TypeAttributes.Public));
        }
    }
