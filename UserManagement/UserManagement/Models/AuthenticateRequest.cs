using System.ComponentModel;

namespace UserManagement.Models
{
    public class AuthenticateRequest
    {
        [DefaultValue("System")]
        public required string Email { get; set; }

        [DefaultValue("System")]
        public required string Password { get; set; }
    }
}