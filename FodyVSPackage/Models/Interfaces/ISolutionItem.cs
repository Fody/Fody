namespace FodyVSPackage.Models
{
    public interface ISolutionItem
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the parent id.
        /// </summary>
        string ParentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the root project.
        /// </summary>
        string Name { get; set; }
    }
}