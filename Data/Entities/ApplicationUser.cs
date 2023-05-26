using CallMaster.Core.RepositoryModule.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace SampleApp.Data.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public DateTime CreatedAt { get; set; }

        public string Ord { get; set; }

        public List<FileInfo> Files { get; set; }

        public virtual ICollection<ApplicationUserClaim> Claims { get; set; }

        public virtual ICollection<ApplicationUserLogin> Logins { get; set; }

        public virtual ICollection<ApplicationUserToken> Tokens { get; set; }

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}