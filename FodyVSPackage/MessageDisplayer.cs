using Microsoft.VisualStudio.Shell;

public class MessageDisplayer
{
    ErrorListProvider errorProvider;


    public MessageDisplayer()
    {
    }

    public MessageDisplayer(ErrorListProvider errorProvider)
    {
        this.errorProvider = errorProvider;
    }

    public void ShowError(string error)
    {
        var errorTask = new ErrorTask
                            {
                                Category = TaskCategory.Misc,
                                Text = error,
                                CanDelete = true,
                            };
        errorProvider.Tasks.Add(errorTask);
    }

    public void ShowInfo(string info)
    {
        var task = new Task
                       {
                           Category = TaskCategory.Misc,
                           Text = info,
                           CanDelete = true,
                       };
        errorProvider.Tasks.Add(task);
    }

}