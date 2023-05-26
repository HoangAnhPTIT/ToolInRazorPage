using Microsoft.AspNetCore.Identity;
using SampleApp.Data.Entities;
using System;

namespace CallMaster.Core.RepositoryModule.Entities
{
    public class ApplicationUserToken : IdentityUserToken<Guid>
    {
        public virtual ApplicationUser User { get; set; }
    }
}