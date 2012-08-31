using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Catel;
using Catel.IoC;
using FodyVSPackage.Models;

namespace FodyVSPackage.Services
{
    public class FodyAddinManager : IFodyAddinManager
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly IVisualStudioService _visualStudioService;

        private readonly Dictionary<string, List<IFodyAddin>> _addins = new Dictionary<string, List<IFodyAddin>>();
        
        public FodyAddinManager(IServiceLocator serviceLocator, IVisualStudioService visualStudioService)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);
            Argument.IsNotNull("visualStudioService", visualStudioService);

            _serviceLocator = serviceLocator;
            _visualStudioService = visualStudioService;
        }

        public IEnumerable<IFodyAddin> GetAddins()
        {
            return GetCurrentSolutionList();
        }

        public IFodyAddin GetAddin(string name)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            return GetAddins().FirstOrDefault(x => string.Equals(x.Name, name));
        }

        public void ReloadAddins()
        {
            _addins.Clear();
        }

        private List<IFodyAddin> GetCurrentSolutionList()
        {
            var visualStudioSolution = _serviceLocator.ResolveType<IVisualStudioService>();
            var currentSolution = visualStudioSolution.GetCurrentSolution();
            if (currentSolution == null)
            {
                return new List<IFodyAddin>();
            }

            var solutionName = currentSolution.FullName;
            if (!_addins.ContainsKey(solutionName))
            {
                string solutionDirectory = Path.GetDirectoryName(currentSolution.FileName);
                _addins[solutionName] = new List<IFodyAddin>(GetFodyAddins(solutionDirectory));
            }

            return _addins[solutionName];
        }

        private IEnumerable<IFodyAddin> GetFodyAddins(string solutionDirectory)
        {
            Argument.IsNotNullOrWhitespace("solutionDirectory", solutionDirectory);

            string packagesDirectory = solutionDirectory;

            var nugetConfig = Path.Combine(solutionDirectory, "NuGet.config");
            if (File.Exists(nugetConfig))
            {
                var config = XDocument.Load(nugetConfig);
                var settings = config.Element("settings");
                if (settings != null)
                {
                    var repositoryPath = settings.Element("repositoryPath");
                    if (repositoryPath != null)
                    {
                        packagesDirectory = Catel.IO.Path.GetFullPath(repositoryPath.Value, solutionDirectory);
                    }
                }
            }

            var addins = new List<IFodyAddin>();

            var directories = Directory.GetDirectories(packagesDirectory, "*.Fody.*", SearchOption.AllDirectories).OrderByDescending(x => x);
            foreach (var fodyAddinDirectory in directories)
            {
                var directoryInfo = new DirectoryInfo(fodyAddinDirectory);

                var addinName = directoryInfo.Name;
                int fodyIndex = addinName.IndexOf(".Fody");
                if (fodyIndex > 0)
                {
                    addinName = addinName.Substring(0, fodyIndex);
                }

                // It might be possible that there are multiple versions, it doesn't matter which one we pick, but we always pick the 
                // first, which is the latest because we sorted the directories descending
                if (!addins.Any(x => string.Equals(x.Name, addinName)))
                {
                    addins.Add(new FodyAddin(addinName, fodyAddinDirectory));    
                }
            }

            return addins.OrderBy(x => x.Name);
        }
    }
}