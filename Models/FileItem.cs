using SampleApp.Models.Enums;

namespace SampleApp.Models
{
    public class FileItem
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public long FileLength { get; set; }  

        public bool IsSelected { get; set; }

        public FileDirection FileDirection { get; set; }
    }
}
