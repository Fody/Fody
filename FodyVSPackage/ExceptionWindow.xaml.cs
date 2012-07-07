using System.Diagnostics;
using System.Windows;


public partial class ExceptionWindow
{

    public ExceptionWindow(ExceptionWindowModel model)
    {
        Model = model;
        InitializeComponent();
        DataContext = Model;
    }

    public ExceptionWindowModel Model { get; set; }


    void Copy(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(Model.ExceptionText);
    }


    void Close(object sender, RoutedEventArgs e)
    {
        Close();
    }

    void LaunchIssues(object sender, RoutedEventArgs e)
    {
        Process.Start("http://code.google.com/p/fody/issues/list");
    }
}