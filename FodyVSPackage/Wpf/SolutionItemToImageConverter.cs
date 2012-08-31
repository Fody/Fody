using System;
using System.Windows.Media.Imaging;
using FodyVSPackage.Models;

namespace FodyVSPackage.Wpf
{
    public class SolutionItemToImageConverter : Catel.Windows.Data.Converters.ValueConverterBase
    {
        private static readonly BitmapSource _folderImage = new BitmapImage(new Uri("/FodyVSPackage;component/resources/folder.png", UriKind.RelativeOrAbsolute));
        private static readonly BitmapSource _projectImage = new BitmapImage(new Uri("/FodyVSPackage;component/resources/project.png", UriKind.RelativeOrAbsolute));

        protected override object Convert(object value, Type targetType, object parameter)
        {
            if (value is ISolutionFolder)
            {
                return _folderImage;
            }

            if (value is IProject)
            {
                return _projectImage;
            }

            return null;
        }
    }
}
