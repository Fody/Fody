namespace Fody;

public class UpdateCopyLocalTask : Task
{
    [Required]
    public ITaskItem[] CopyLocalFiles { get; set; } = null!;

    [Output]
    public ITaskItem[] UpdatedCopyLocalFiles { get; set; } = null!;

    [Required]
    public string IntermediateCopyLocalFilesCache { get; set; } = null!;

    public override bool Execute()
    {
        UpdatedCopyLocalFiles = CopyLocalFiles;

        InnerExecute();

        return true;
    }

    void InnerExecute()
    {
        if (string.IsNullOrEmpty(IntermediateCopyLocalFilesCache) || !File.Exists(IntermediateCopyLocalFilesCache))
        {
            return;
        }

        var updatedReferenceCopyLocalPaths = new HashSet<string>(File.ReadAllLines(IntermediateCopyLocalFilesCache), StringComparer.OrdinalIgnoreCase);
        var referenceCopyLocalPaths = new HashSet<string>(CopyLocalFiles.Select(_ => _.ItemSpec), StringComparer.OrdinalIgnoreCase);

        var existingReferenceCopyLocalFiles = CopyLocalFiles
            .Where(item => updatedReferenceCopyLocalPaths.Contains(item.ItemSpec));

        var newReferenceCopyLocalFiles = updatedReferenceCopyLocalPaths
            .Where(item => !referenceCopyLocalPaths.Contains(item))
            .Select(item => new TaskItem(item));

        UpdatedCopyLocalFiles = existingReferenceCopyLocalFiles.Concat(newReferenceCopyLocalFiles).ToArray();
    }
}