using System;

namespace Foxbyte.Core.Services.Permission
{
    public interface IPermissionService
    {
        void DisposeService();
        bool HasPermission(string p);
        bool HasRole(string r);
        void InitService();
        event EventHandler<PermissionService.UserChangedEventArgs> UserChanged;
        event EventHandler UserBecameAnonymous;
        bool AddRole(string role);
        bool RemoveRole(string role);
        bool AddPermission(string permission);
        bool RemovePermission(string permission);
        event EventHandler<PermissionService.RoleChangedEventArgs> RoleAdded;
        event EventHandler<PermissionService.RoleChangedEventArgs> RoleRemoved;
        event EventHandler<PermissionService.PermissionChangedEventArgs> PermissionAdded;
        event EventHandler<PermissionService.PermissionChangedEventArgs> PermissionRemoved;
    }
}