using System;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

[ProvideAutoLoad("F1536EF8-92EC-443C-9ED7-FDADF150DA82")] //SolutionExists
[ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F ")] //NoSolution
[ProvideAutoLoad("4d7a79c7-e2e3-4140-93cc-f0e68a6cae56")]
[PackageRegistration(UseManagedResourcesOnly = true)]
[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[Guid("16486db9-230d-4ab0-bef3-e5f81d4175eb")]
public sealed class FodyVSPackagePackage : Package
{

    protected override void Initialize()
    {
        base.Initialize();
        var exceptionDialog = new ExceptionDialog();
        try
        {
            var menuCommandService = (IMenuCommandService) GetService(typeof (IMenuCommandService));
            var errorListProvider = new ErrorListProvider(ServiceProvider.GlobalProvider);

            var currentProjectFinder = new CurrentProjectFinder();
            var contentsFinder = new ContentsFinder();
            var configureMenuCallback = new ConfigureMenuCallback(currentProjectFinder, contentsFinder, exceptionDialog);
            var messageDisplayer = new MessageDisplayer(errorListProvider);
            var disableMenuConfigure = new DisableMenuConfigure(currentProjectFinder, messageDisplayer, exceptionDialog);
            var containsFodyChecker = new ContainsFodyChecker();
            var menuStatusChecker = new MenuStatusChecker(currentProjectFinder, exceptionDialog, containsFodyChecker);
            new MenuConfigure(configureMenuCallback, disableMenuConfigure, menuCommandService, menuStatusChecker).RegisterMenus();
            var taskFileReplacer = new TaskFileReplacer(messageDisplayer, contentsFinder);
            var taskFileProcessor = new TaskFileProcessor(taskFileReplacer, messageDisplayer);
            var allProjectFinder = new AllProjectFinder();
            var msBuildKiller = new MSBuildKiller();
            new SolutionEvents(taskFileProcessor, exceptionDialog, allProjectFinder, msBuildKiller).RegisterSolutionEvents();
            new TaskFileReplacer(messageDisplayer, contentsFinder).CheckForFilesToUpdate();
        }
        catch (Exception exception)
        {
            exceptionDialog.HandleException(exception);
        }
    }

}