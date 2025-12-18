
using System.Collections.Generic;
using System.Linq;

namespace Foxbyte.Core.Services.Permission
{
    public sealed class RequiredPermissions
    {
        public List<string> Roles = new();
        public List<string> Permissions = new();

        public bool IsAllowed(IPermissionService svc)
        {
            if (svc == null) return true;
            if (Roles.Count == 0 && Permissions.Count == 0) return true;

            bool anyRole = Roles.Count > 0 && Roles.Any(svc.HasRole);
            bool anyPerm = Permissions.Count > 0 && Permissions.Any(svc.HasPermission);
            return anyRole || anyPerm;
        }
    }
}