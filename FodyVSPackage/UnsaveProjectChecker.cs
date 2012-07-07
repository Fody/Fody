using System.Windows;
using EnvDTE;

public static class UnsaveProjectChecker
{
    public static bool HasUnsavedPendingChanges(Project project)
    {
        if (project.Saved)
        {
            return false;
        }
        MessageBox.Show("This action needs to modify your project file. Please save your pending changes and try again.", string.Format("Please save '{0}' first.", project.Name), MessageBoxButton.OK);
        return true;
    }
}