using Microsoft.AspNetCore.Identity;
using System;

namespace CallMaster.Core.RepositoryModule.Entities
{
    public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
    {
        public virtual ApplicationRole Role { get; set; }
    }
}