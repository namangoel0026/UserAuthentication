using System.Text.Json.Serialization;

using System.Data;

namespace UserManagement.Models
{
    public class User
    {
        public int UserId {  get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public ICollection<Role> Roles { get; set; }
        public string Password { get; set; }
        public bool isActive { get; set; }
    }
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool isActive { get; set; }
    }
}
