using System;
using System.Windows;
using Catel;
using Catel.IoC;
using Catel.Logging;
using Catel.MVVM;
using EnvDTE;
using FodyVSPackage.Services;

namespace FodyVSPackage
{
    public static class CatelInitializer
    {
        public static void Initialize(DTE dte)
        {
            Argument.IsNotNull("dte", dte);

            var currentApplication = Application.Current;
            if (currentApplication == null)
            {
                currentApplication = new Application();
            }

            currentApplication.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Catel.Extensions.Controls;component/themes/generic.xaml", UriKind.RelativeOrAbsolute) });

#if DEBUG
            LogManager.RegisterDebugListener();
#endif

            Catel.Environment.BypassDevEnvCheck = true;

            var serviceLocator = ServiceLocator.Instance;
            serviceLocator.RegisterInstance(serviceLocator);
            serviceLocator.RegisterInstance<IVisualStudioService>(new VisualStudioService(dte));

            serviceLocator.RegisterType<IFodyAddinManager, FodyAddinManager>();
            serviceLocator.RegisterType<IProjectManager, ProjectManager>();

            ViewModelServiceHelper.RegisterDefaultViewModelServices(serviceLocator);
        }
    }
}