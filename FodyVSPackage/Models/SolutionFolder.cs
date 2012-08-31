namespace FodyVSPackage.Models
{
    public class SolutionFolder : SolutionItem, ISolutionFolder
    {
        public SolutionFolder(string id, string name, string parentId = null)
            : base(id, name, parentId)
        {
            
        }
    }
}