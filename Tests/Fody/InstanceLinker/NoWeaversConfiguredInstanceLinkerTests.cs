﻿using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;

public class NoWeaversConfiguredInstanceLinkerTests : TestBase
{
    [Fact]
    public void Should_add_weavers_project_When_weavers_project_found_and_not_used_by_configured_weavers()
    {
        var mock = new Mock<Processor>();
        mock.Setup(x => x.WeaverProjectContainsType("ModuleWeaver"))
            .Returns(true);
        mock.CallBase = true;
        var processor = mock.Object;

        processor.WeaverAssemblyPath = "Path";
        processor.FoundWeaverProjectFile = true;
        processor.Weavers = new List<WeaverEntry>();
        processor.Logger = new Mock<BuildLogger>().Object;

        processor.ConfigureWhenNoWeaversFound();

        var weaverEntry = processor.Weavers.First();
        Assert.Equal("ModuleWeaver",weaverEntry.TypeName);
        Assert.Equal("Path",weaverEntry.AssemblyPath);
    }
}