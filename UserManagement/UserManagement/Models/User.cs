using System.Text.Json.Serialization;

using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Models
{
    
        public class UserModel
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }

            [Required]
            public string Email { get; set; }
            [Required]
            public string UserRoles { get; set; } 
            public bool IsActive { get; set; }
    }

        public class RoleModel
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [Required]
            [MaxLength(100)]
            public string Name { get; set; }

            [Required]
            [MaxLength(500)]
            public string Description { get; set; }          
            public bool IsActive { get; set; }
    }
        public class UserRoleModel
        {
            public int UserId { get; set; }
            public UserModel User { get; set; }

            public int RoleId { get; set; }
            public RoleModel Role { get; set; }
        }
        public class UserRequest
        {
            [Required]
            public int Id {  get; set; }
            [Required]
            public string Username { get; set; }

            [Required]
            public string Email { get; set; }

            [Required]
            public string UserRoles { get; set; }
        }

    
}
