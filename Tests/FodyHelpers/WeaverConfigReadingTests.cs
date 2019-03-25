using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fody;
using ObjectApproval;
using Xunit;

public class WeaverConfigReadingTests
{
    [Fact]
    public void NoValues()
    {
        var weaver = new Weaver
        {
            Config = XElement.Parse("<Node/>")
        };
        Verify(weaver);
    }

    [Fact]
    public void UseExisting()
    {
        var weaver = new Weaver
        {
            Config = XElement.Parse("<Node/>"),
            BoolMember = true,
            StringMember = "foo"
        };
        Verify(weaver);
    }

    [Fact]
    public void WithValues()
    {
        var weaver = new Weaver
        {
            Config = XElement.Parse("<Node BoolMember='true' StringMember='Foo'/>")
        };
        Verify(weaver);
    }

    static void Verify(Weaver weaver)
    {
        weaver.Execute();
        ObjectApprover.VerifyWithJson(
            new
            {
                weaver.BoolMember,
                weaver.StringMember
            });
    }

    public class Weaver : BaseModuleWeaver
    {
        public override void Execute()
        {
            AssignFromConfig(() => BoolMember);
            AssignFromConfig(() => StringMember);
        }

        public bool BoolMember;
        public string StringMember;

        public override bool ShouldCleanReference => true;

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            return Enumerable.Empty<string>();
        }
    }
}