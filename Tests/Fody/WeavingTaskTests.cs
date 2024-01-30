using Microsoft.Build.Utilities;

public class WeavingTaskTests
{
    [Fact]
    public void GetWeaversFromProps()
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
        Assert.Equal(5, weavers.Count);

        var weaverA = weavers[0];
        Assert.Equal(@"SampleWeaverDir\SampleWeaverA.Fody.dll", weaverA.AssemblyPath);
        Assert.Equal(@"SampleWeaverA.Fody", weaverA.WeaverName);
        Assert.Equal(@"SampleWeaverA", weaverA.ElementName);
        Assert.Equal(@"ModuleWeaver", weaverA.TypeName);
        Assert.True(weaverA.HasPackageReference);
        Assert.Equal("", weaverA.PrivateAssets);
        Assert.Equal("", weaverA.IncludeAssets);

        var weaverB = weavers[1];
        Assert.Equal(@"SampleWeaverDir\SampleWeaverB.Fody.dll", weaverB.AssemblyPath);
        Assert.Equal(@"SampleWeaverB.Fody", weaverB.WeaverName);
        Assert.Equal(@"SampleWeaverB", weaverB.ElementName);
        Assert.Equal(@"ModuleWeaver", weaverB.TypeName);
        Assert.True(weaverB.HasPackageReference);
        Assert.Equal("all", weaverB.PrivateAssets);
        Assert.Equal("compile", weaverB.IncludeAssets);

        var weaverC = weavers[2];
        Assert.Equal(@"SampleWeaverDir\SampleWeaverC.Fody.dll", weaverC.AssemblyPath);
        Assert.Equal(@"SampleWeaverC.Fody", weaverC.WeaverName);
        Assert.Equal(@"SampleWeaverC", weaverC.ElementName);
        Assert.Equal(@"ModuleWeaver", weaverC.TypeName);
        Assert.False(weaverC.HasPackageReference);
        Assert.Null(weaverC.PrivateAssets);
        Assert.Null(weaverC.IncludeAssets);

        var weaverD1 = weavers[3];
        Assert.Equal(@"SampleWeaverDir\SampleWeaverD.Fody.dll", weaverD1.AssemblyPath);
        Assert.Equal(@"SampleWeaverD.Fody", weaverD1.WeaverName);
        Assert.Equal(@"WeaverDClass1", weaverD1.ElementName);
        Assert.Equal(@"WeaverDClass1", weaverD1.TypeName);
        Assert.False(weaverD1.HasPackageReference);
        Assert.Null(weaverD1.PrivateAssets);
        Assert.Null(weaverD1.IncludeAssets);

        var weaverD2 = weavers[4];
        Assert.Equal(@"SampleWeaverDir\SampleWeaverD.Fody.dll", weaverD2.AssemblyPath);
        Assert.Equal(@"SampleWeaverD.Fody", weaverD2.WeaverName);
        Assert.Equal(@"WeaverDClass2", weaverD2.ElementName);
        Assert.Equal(@"WeaverDClass2", weaverD2.TypeName);
        Assert.False(weaverD2.HasPackageReference);
        Assert.Null(weaverD2.PrivateAssets);
        Assert.Null(weaverD2.IncludeAssets);
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
