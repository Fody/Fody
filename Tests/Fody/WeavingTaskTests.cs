using Task = System.Threading.Tasks.Task;
using Microsoft.Build.Utilities;

public class WeavingTaskTests
{
    [Test]
    public async Task GetWeaversFromProps()
    {
        var task = new WeavingTask
        {
            WeaverFiles =
            [
                new TaskItem(@"SampleWeaverDir\SampleWeaverA.Fody.dll"),
                new TaskItem(@"SampleWeaverDir\SampleWeaverB.Fody.dll"),
                new TaskItem(@"SampleWeaverDir\SampleWeaverC.Fody.dll"),
                new TaskItem(@"SampleWeaverDir\SampleWeaverD.Fody.dll", Metadata(("WeaverClassNames", "WeaverDClass1;WeaverDClass2")))
            ],
            PackageReferences =
            [
                new TaskItem("SampleWeaverA.Fody"),
                new TaskItem("SampleWeaverB.Fody", Metadata(("PrivateAssets", "all"), ("IncludeAssets", "compile")))
            ]
        };

        var weavers = task.GetWeaversFromProps();
        await Assert.That(weavers.Count).IsEqualTo(5);

        var weaverA = weavers[0];
        await Assert.That(weaverA.AssemblyPath).IsEqualTo(@"SampleWeaverDir\SampleWeaverA.Fody.dll");
        await Assert.That(weaverA.WeaverName).IsEqualTo(@"SampleWeaverA.Fody");
        await Assert.That(weaverA.ElementName).IsEqualTo(@"SampleWeaverA");
        await Assert.That(weaverA.TypeName).IsEqualTo(@"ModuleWeaver");
        await Assert.That(weaverA.HasPackageReference).IsTrue();
        await Assert.That(weaverA.PrivateAssets).IsEqualTo("");
        await Assert.That(weaverA.IncludeAssets).IsEqualTo("");

        var weaverB = weavers[1];
        await Assert.That(weaverB.AssemblyPath).IsEqualTo(@"SampleWeaverDir\SampleWeaverB.Fody.dll");
        await Assert.That(weaverB.WeaverName).IsEqualTo(@"SampleWeaverB.Fody");
        await Assert.That(weaverB.ElementName).IsEqualTo(@"SampleWeaverB");
        await Assert.That(weaverB.TypeName).IsEqualTo(@"ModuleWeaver");
        await Assert.That(weaverB.HasPackageReference).IsTrue();
        await Assert.That(weaverB.PrivateAssets).IsEqualTo("all");
        await Assert.That(weaverB.IncludeAssets).IsEqualTo("compile");

        var weaverC = weavers[2];
        await Assert.That(weaverC.AssemblyPath).IsEqualTo(@"SampleWeaverDir\SampleWeaverC.Fody.dll");
        await Assert.That(weaverC.WeaverName).IsEqualTo(@"SampleWeaverC.Fody");
        await Assert.That(weaverC.ElementName).IsEqualTo(@"SampleWeaverC");
        await Assert.That(weaverC.TypeName).IsEqualTo(@"ModuleWeaver");
        await Assert.That(weaverC.HasPackageReference).IsFalse();
        await Assert.That(weaverC.PrivateAssets).IsNull();
        await Assert.That(weaverC.IncludeAssets).IsNull();

        var weaverD1 = weavers[3];
        await Assert.That(weaverD1.AssemblyPath).IsEqualTo(@"SampleWeaverDir\SampleWeaverD.Fody.dll");
        await Assert.That(weaverD1.WeaverName).IsEqualTo(@"SampleWeaverD.Fody");
        await Assert.That(weaverD1.ElementName).IsEqualTo(@"WeaverDClass1");
        await Assert.That(weaverD1.TypeName).IsEqualTo(@"WeaverDClass1");
        await Assert.That(weaverD1.HasPackageReference).IsFalse();
        await Assert.That(weaverD1.PrivateAssets).IsNull();
        await Assert.That(weaverD1.IncludeAssets).IsNull();

        var weaverD2 = weavers[4];
        await Assert.That(weaverD2.AssemblyPath).IsEqualTo(@"SampleWeaverDir\SampleWeaverD.Fody.dll");
        await Assert.That(weaverD2.WeaverName).IsEqualTo(@"SampleWeaverD.Fody");
        await Assert.That(weaverD2.ElementName).IsEqualTo(@"WeaverDClass2");
        await Assert.That(weaverD2.TypeName).IsEqualTo(@"WeaverDClass2");
        await Assert.That(weaverD2.HasPackageReference).IsFalse();
        await Assert.That(weaverD2.PrivateAssets).IsNull();
        await Assert.That(weaverD2.IncludeAssets).IsNull();
    }

    static IDictionary Metadata(params (string key, string value)[] items)
    {
        var result = new Dictionary<string, string>();

        foreach (var (key, value) in items)
        {
            result.Add(key, value);
        }

        return result;
    }
}
