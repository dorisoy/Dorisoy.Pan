using System;

namespace Dorisoy.Pan.Data.Dto
{
    public class DownloadDocumentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FolderPath { get; set; }
        public string OriginalFolderPath { get; set; }
        public string Path { get; set; }
        public Guid FolderId { get; set; }
    }
}
