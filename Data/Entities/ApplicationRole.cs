using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace CallMaster.Core.RepositoryModule.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string Description { get; set; }

        public string Title { get; set; }

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }

    }
}