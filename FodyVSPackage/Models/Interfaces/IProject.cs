using System.Collections.Generic;

namespace FodyVSPackage.Models
{
    public interface IProject : ISolutionItem
    {
        /// <summary>
        /// Gets or sets the project directory.
        /// </summary>
        string Directory { get; set; }

        string FodyConfig { get; }

        /// <summary>
        /// Gets the list of addins for this project.
        /// </summary>
        List<IFodyAddin> Addins { get; set; }
    }
}