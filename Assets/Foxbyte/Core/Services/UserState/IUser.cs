using System.Collections.Generic;

namespace Foxbyte.Core.Services.UserState
{
    public interface IUser
    {
        string Id { get; }
        string DisplayName { get; }
        string Email { get; }
        string Image { get; }
        HashSet<string> Roles { get; }
        HashSet<string> Permissions { get; }
    }
}