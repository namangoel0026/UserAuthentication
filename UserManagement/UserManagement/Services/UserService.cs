using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.Models;
namespace UserManagement.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly UserContext db;

        public UserService(IConfiguration configuration, UserContext _db)
        {
            _configuration = configuration;
            db = _db;
        }

       

        public async Task<IEnumerable<User>> GetAll()
        {
            return await db.Users.Where(x => x.isActive == true).ToListAsync();
        }

        public async Task<User?> GetById(int id)
        {
            return await db.Users.FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<User?> AddAndUpdateUser(User userObj)
        {
            bool isSuccess = false;
            if (userObj.UserId > 0)
            {
                var obj = await db.Users.FirstOrDefaultAsync(c => c.UserId == userObj.UserId);
                if (obj != null)
                {
                    // obj.Address = userObj.Address;
                    obj.Username = userObj.Username;
                    db.Users.Update(obj);
                    isSuccess = await db.SaveChangesAsync() > 0;
                }
            }
            else
            {
                await db.Users.AddAsync(userObj);
                isSuccess = await db.SaveChangesAsync() > 0;
            }

            return isSuccess ? userObj : null;
        }
        
    }
}
