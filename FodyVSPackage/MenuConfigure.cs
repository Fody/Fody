using System;
using System.ComponentModel.Design;
using Catel.IoC;
using Catel.MVVM.Services;
using EnvDTE;
using FodyVSPackage;
using FodyVSPackage.ViewModels;
using Microsoft.VisualStudio.Shell;

public class MenuConfigure
{
    OleMenuCommand configureCommand;
    OleMenuCommand disableCommand;
    OleMenuCommand manageCommand;

    DTE _dte;
    ConfigureMenuCallback configureMenuCallback;
    DisableMenuConfigure disableMenuConfigure;
    IMenuCommandService menuCommandService;
    MenuStatusChecker menuStatusChecker;
    Guid cmdSet = new Guid("c0886fa2-b3c2-4546-9cd1-ed18d53cab98");

    private bool _isCatelInitialized;

    public MenuConfigure(DTE dte, ConfigureMenuCallback configureMenuCallback, DisableMenuConfigure disableMenuConfigure, IMenuCommandService menuCommandService, MenuStatusChecker menuStatusChecker)
    {
        _dte = dte;
        this.configureMenuCallback = configureMenuCallback;
        this.disableMenuConfigure = disableMenuConfigure;
        this.menuCommandService = menuCommandService;
        this.menuStatusChecker = menuStatusChecker;
    }

    public void RegisterMenus()
    {
        CreateConfigCommand();
        CreateDisableCommand();
        CreateManageCommand();
    }

    void CreateConfigCommand()
    {
        var configureCommandId = new CommandID(cmdSet, 1);
        configureCommand = new OleMenuCommand(delegate { configureMenuCallback.ConfigureCallback(); }, configureCommandId);
        configureCommand.BeforeQueryStatus += delegate { menuStatusChecker.ConfigureCommandStatusCheck(configureCommand); };
        menuCommandService.AddCommand(configureCommand);
    }

    void CreateDisableCommand()
    {
        var disableCommandId = new CommandID(cmdSet, 2);
        disableCommand = new OleMenuCommand(delegate { disableMenuConfigure.DisableCallback(); }, disableCommandId)
        {
            Enabled = false
        };
        disableCommand.BeforeQueryStatus += delegate { menuStatusChecker.DisableCommandStatusCheck(disableCommand); };
        menuCommandService.AddCommand(disableCommand);
    }

    void CreateManageCommand()
    {
        var manageCommandId = new CommandID(cmdSet, 3);
        manageCommand = new OleMenuCommand(delegate
        {
            if (!_isCatelInitialized)
            {
                CatelInitializer.Initialize(_dte);
                _isCatelInitialized = true;
            }

            var uiVisualizerService = ServiceLocator.Instance.ResolveType<IUIVisualizerService>();
            var vm = DependencyInjectionHelper.CreateInstance<ManageAddinsViewModel>();

            uiVisualizerService.ShowDialog(vm);
        }, manageCommandId)
        {
            Enabled = true
        };
        menuCommandService.AddCommand(manageCommand);
    }
}