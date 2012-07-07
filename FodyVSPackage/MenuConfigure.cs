using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

public class MenuConfigure
{
    OleMenuCommand configureCommand;
    OleMenuCommand disableCommand;
    ConfigureMenuCallback configureMenuCallback;
    DisableMenuConfigure disableMenuConfigure;
    IMenuCommandService menuCommandService;
    MenuStatusChecker menuStatusChecker;
    Guid cmdSet = new Guid("c0886fa2-b3c2-4546-9cd1-ed18d53cab98");

    public MenuConfigure(ConfigureMenuCallback configureMenuCallback, DisableMenuConfigure disableMenuConfigure, IMenuCommandService menuCommandService, MenuStatusChecker menuStatusChecker)
    {
        this.configureMenuCallback = configureMenuCallback;
        this.disableMenuConfigure = disableMenuConfigure;
        this.menuCommandService = menuCommandService;
        this.menuStatusChecker = menuStatusChecker;
    }

    public void RegisterMenus()
    {
        CreateConfigCommand();
        CreateDisableCommand();
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

    void CreateConfigCommand()
    {
        var configureCommandId = new CommandID(cmdSet, 1);
        configureCommand = new OleMenuCommand(delegate { configureMenuCallback.ConfigureCallback(); }, configureCommandId);
        configureCommand.BeforeQueryStatus += delegate { menuStatusChecker.ConfigureCommandStatusCheck(configureCommand); };
        menuCommandService.AddCommand(configureCommand);
    }
}