using System;

namespace SampleApp.Data.Entities
{
    public class FileInfo
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}