using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Fody;

using Mono.Cecil;

using Xunit;

#pragma warning disable CS0618 // Type or member is obsolete

public class ImportExtensionsTests : TestBase
{
    [Fact]
    public void Run()
    {
        var assemblyPath = Path.Combine(CodeBaseLocation.CurrentDirectory, "DummyAssembly.dll");
        var weaver = new TestWeaver();
        weaver.ExecuteTestRun(assemblyPath);

        Assert.Equal("System.Boolean System.String::Equals(System.String,System.String,System.StringComparison)", weaver.StringEquals.FullName);
        Assert.Equal("System.String", weaver.StringType.FullName);
        Assert.Equal("System.Reflection.PropertyInfo System.Type::GetProperty(System.String,System.Reflection.BindingFlags)", weaver.GetPropertyInfo.FullName);
    }

    class TestWeaver : BaseModuleWeaver
    {
        public MethodReference StringEquals { get; set; }

        public TypeReference StringType { get; set; }

        public MethodReference GetPropertyInfo { get; set; }

        public override void Execute()
        {
#pragma warning disable CS1720 // Expression will always cause a System.NullReferenceException because the type's default value is null
            StringType = this.ImportType<string>();
            StringEquals = this.ImportMethod(() => string.Equals(default, default, default));
            GetPropertyInfo = this.TryImportMethod(() => default(Type).GetProperty(default, default(BindingFlags)));
#pragma warning restore CS1720 // Expression will always cause a System.NullReferenceException because the type's default value is null
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            return new[] { "mscorlib", "System", "System.Reflection", "System.Runtime", "netstandard" };
        }
    }
}
