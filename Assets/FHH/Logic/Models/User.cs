using System;
using System.Collections.Generic;
using System.Linq;
using Foxbyte.Core.Services.UserState;

namespace FHH.Logic.Models
{
    public class User :IUser
    {
        public string Id { get; }
        public string DisplayName { get; }
        public string Email { get; }
        public string Image { get; }
        public HashSet<string> Roles { get; }
        public HashSet<string> Permissions { get; }
        
        public User(
            string displayName,
            string id = null,
            string email = null,
            string image = null,
            IEnumerable<string> roles = null,
            IEnumerable<string> permissions = null)
        {
            DisplayName = displayName;
            Id = id;
            Email = email;
            Roles = roles != null ? new HashSet<string>(roles) : new HashSet<string>();
            Permissions = permissions != null ? new HashSet<string>(permissions) : new HashSet<string>();
            Image = image;
        }

        public static User Anonymous(string name = "Anonymous") => new User(name);

        public static User TestUser(string name = "Test User", string id = "test123", string email = "test@testdomain.com") => 
            new User(name, id, email, string.Empty, new[] { "test-role" }, new[] { "test-permission" });

        public User(UserInfoDto dto)
        {
            //Id = dto.Id;
            Id = Guid.NewGuid().ToString(); 
            DisplayName = dto.Name;
            Email = dto.Email;
            Image = dto.Image;
            Roles = new HashSet<string>();
            Permissions = new HashSet<string>();
        }
    }
}