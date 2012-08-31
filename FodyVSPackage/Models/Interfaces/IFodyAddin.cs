namespace FodyVSPackage.Models
{
    public interface IFodyAddin
    {
        /// <summary>
        /// Gets the name of the addin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the directory name of the addin.
        /// </summary>
        string Directory { get; }
    }
}