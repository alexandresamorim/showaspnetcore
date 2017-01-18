using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.MongoDB;

namespace Showaspnetcore.Data
{
    public class Role : IdentityRole
    {
        public Role() : base()
        {
            RolePermissions = new HashSet<RolePermission>();
        }

        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
