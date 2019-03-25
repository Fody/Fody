using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fody;
using ObjectApproval;
using Xunit;

public class WeaverConfigReadingTests
{
    [Fact]
    public void WithValues()
    {
        var weaver = new Weaver
        {
            Config = XElement.Parse("<Node BoolMember='true'/>")
        };
        weaver.Execute();
        ObjectApprover.VerifyWithJson(new {weaver.BoolMember});
    }

    public class Weaver : BaseModuleWeaver
    {
        public override void Execute()
        {
            AssignFromConfig(()=>BoolMember);
        }

        public bool BoolMember;

        public override bool ShouldCleanReference => true;

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            return Enumerable.Empty<string>();
        }
    }
}