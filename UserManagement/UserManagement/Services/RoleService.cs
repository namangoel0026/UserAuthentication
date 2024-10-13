using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.Models;
namespace UserManagement.Services
{
    public class RoleService : IRoleService
    {
        private readonly IConfiguration _configuration;
        private readonly UserContext db;

        public RoleService(IConfiguration configuration, UserContext _db)
        {
            _configuration = configuration;
            db = _db;
        }

       public async Task<IEnumerable<RoleModel>> GetAll()
        {
            return await db.Roles.Where(x => x.IsActive == true).ToListAsync();
        }

        public async Task<RoleModel?> GetById(int id)
        {
            return await db.Roles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<RoleModel?> AddAndUpdateRole(RoleModel RoleObj)
        {
            try
            {
                bool isSuccess = false;
                if (RoleObj.Id > 0)
                {
                    var obj = await db.Roles.FirstOrDefaultAsync(c => c.Id == RoleObj.Id);
                    if (obj != null)
                    {
                        // obj.Address = RoleObj.Address;
                        obj.Name = RoleObj.Name;
                        db.Roles.Update(obj);
                        isSuccess = await db.SaveChangesAsync() > 0;
                    }
                }
                else
                {
                    await db.Roles.AddAsync(RoleObj);
                    isSuccess = await db.SaveChangesAsync() > 0;
                }

                return isSuccess ? RoleObj : null;
            }
            catch (DbUpdateException ex)
            {
                // Handle the exception, log it, or return a UserModel-friendly message.
                if (ex.InnerException?.Message.Contains("unique constraint") == true)
                {
                    throw new Exception("A RoleModel with the same name or description already exists.");
                }

                throw; 
            }
        }
    }
}
