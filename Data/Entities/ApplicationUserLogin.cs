using Microsoft.AspNetCore.Identity;
using SampleApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CallMaster.Core.RepositoryModule.Entities
{
    public class ApplicationUserLogin : IdentityUserLogin<Guid>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
