using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Fody
{
    public class UpdateReferenceCopyLocalTask : Task
    {
        [Required]
        public ITaskItem[] ReferenceCopyLocalFiles { get; set; } = null!;

        [Output]
        public ITaskItem[] UpdatedReferenceCopyLocalFiles { get; set; } = null!;

        [Required]
        public string IntermediateCopyLocalFilesCache { get; set; } = null!;

        public override bool Execute()
        {
            UpdatedReferenceCopyLocalFiles = ReferenceCopyLocalFiles;

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
            var referenceCopyLocalPaths = new HashSet<string>(ReferenceCopyLocalFiles.Select(x => x.ItemSpec), StringComparer.OrdinalIgnoreCase);

            var existingReferenceCopyLocalFiles = ReferenceCopyLocalFiles
                .Where(item => updatedReferenceCopyLocalPaths.Contains(item.ItemSpec));

            var newReferenceCopyLocalFiles = updatedReferenceCopyLocalPaths
                .Where(item => !referenceCopyLocalPaths.Contains(item))
                .Select(item => new TaskItem(item));

            UpdatedReferenceCopyLocalFiles = existingReferenceCopyLocalFiles.Concat(newReferenceCopyLocalFiles).ToArray();
        }
    }
}