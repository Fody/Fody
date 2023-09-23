using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

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
            .Where(_ => updatedReferenceCopyLocalPaths.Contains(_.ItemSpec));

        var newReferenceCopyLocalFiles = updatedReferenceCopyLocalPaths
            .Where(_ => !referenceCopyLocalPaths.Contains(_))
            .Select(_ => new TaskItem(_));

        UpdatedCopyLocalFiles = existingReferenceCopyLocalFiles.Concat(newReferenceCopyLocalFiles).ToArray();
    }
}