using Foxbyte.Core.Services.UserState;
using System;
using System.Collections.Generic;

namespace Foxbyte.Core.Services.Permission
{
    /// <summary>
    /// Use this service to check if a user has a specific permission or role.
    /// InitService will be automatically called by the service locator.
    /// A new PermissionService should be created for each user session
    /// by passing the current user's roles and permissions.
    /// </summary>
    public class PermissionService : IAppService, IPermissionService
    {
        public event EventHandler<UserChangedEventArgs> UserChanged;
        public event EventHandler UserBecameAnonymous;

        public event EventHandler<RoleChangedEventArgs> RoleAdded;
        public event EventHandler<RoleChangedEventArgs> RoleRemoved;
        public event EventHandler<PermissionChangedEventArgs> PermissionAdded;
        public event EventHandler<PermissionChangedEventArgs> PermissionRemoved;

        private HashSet<string> _roles;
        private HashSet<string> _permissions;
        private IUser _user;

        public void InitService()
        {
            //ULog.Info("[PermissionService] initialized.");
        }

        public void DisposeService()
        {
            //ULog.Info("[PermissionService] disposed.");
        }

        public PermissionService(IUser user)
        {
            // build roles
            if (user.Roles != null)
            {
                _roles = new HashSet<string>(user.Roles, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                _roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            // build permissions
            if (user.Permissions != null)
            {
                _permissions = new HashSet<string>(user.Permissions, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                _permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            _user = user;
        }

        public bool HasRole(string r) => _roles.Contains(r);
        public bool HasPermission(string p) => _permissions.Contains(p);


        public void SetUser(IUser user)
        {
            var oldUser = _user;
            _roles.Clear();
            _permissions.Clear();
            if (user.Roles != null)
            {
                _roles = new HashSet<string>(user.Roles, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                _roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            if (user.Permissions != null)
            {
                _permissions = new HashSet<string>(user.Permissions, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                _permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            _user = user;
            OnUserChanged(oldUser, _user);
            if (IsAnonymous())
            {
                OnUserBecameAnonymous();
            }
        }

        public IUser GetUser()
        {
            return _user;
        }

        public bool IsAnonymous()
        {
            return _user.DisplayName.Equals("Anonymous");
        }

        /// <summary>
        /// Is one of the user's roles moderator?
        /// </summary>
        /// <returns></returns>
        public bool IsModerator()
        {
            foreach (var role in _roles)
            {
                if (string.Equals(role, "moderator", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsOwner()
        {
            foreach (var role in _roles)
            {
                if (string.Equals(role, "owner", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Is guest the only role assigned to the user?
        /// </summary>
        /// <returns></returns>
        public bool IsGuest()
        {
            if (_roles == null || _roles.Count != 1)
            {
                return false;
            }

            foreach (var role in _roles)
            {
                return string.Equals(role, "guest", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public sealed class UserChangedEventArgs : EventArgs
        {
            public IUser OldUser { get; }
            public IUser NewUser { get; }

            public UserChangedEventArgs(IUser oldUser, IUser newUser)
            {
                OldUser = oldUser;
                NewUser = newUser;
            }
        }
        
        private void OnUserChanged(IUser oldUser, IUser newUser)
        {
            var handler = UserChanged;
            if (handler != null)
            {
                handler(this, new UserChangedEventArgs(oldUser, newUser));
            }
        }

        private void OnUserBecameAnonymous()
        {
            var handler = UserBecameAnonymous;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public sealed class RoleChangedEventArgs : EventArgs
        {
            public string Role { get; }
            public RoleChangedEventArgs(string role) { Role = role; }
        }

        public sealed class PermissionChangedEventArgs : EventArgs
        {
            public string Permission { get; }
            public PermissionChangedEventArgs(string permission) { Permission = permission; }
        }

        public bool AddRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) { return false; }
            if (_roles.Add(role))
            {
                OnRoleAdded(role);
                return true;
            }
            return false;
        }

        public bool RemoveRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) { return false; }
            if (_roles.Remove(role))
            {
                OnRoleRemoved(role);
                return true;
            }
            return false;
        }

        public bool AddPermission(string permission)
        {
            if (string.IsNullOrWhiteSpace(permission)) { return false; }
            if (_permissions.Add(permission))
            {
                OnPermissionAdded(permission);
                return true;
            }
            return false;
        }

        public bool RemovePermission(string permission)
        {
            if (string.IsNullOrWhiteSpace(permission)) { return false; }
            if (_permissions.Remove(permission))
            {
                OnPermissionRemoved(permission);
                return true;
            }
            return false;
        }

        private void OnRoleAdded(string role)
        {
            var handler = RoleAdded;
            if (handler != null) { handler(this, new RoleChangedEventArgs(role)); }
        }

        private void OnRoleRemoved(string role)
        {
            var handler = RoleRemoved;
            if (handler != null) { handler(this, new RoleChangedEventArgs(role)); }
        }

        private void OnPermissionAdded(string permission)
        {
            var handler = PermissionAdded;
            if (handler != null) { handler(this, new PermissionChangedEventArgs(permission)); }
        }

        private void OnPermissionRemoved(string permission)
        {
            var handler = PermissionRemoved;
            if (handler != null) { handler(this, new PermissionChangedEventArgs(permission)); }
        }
    }
}