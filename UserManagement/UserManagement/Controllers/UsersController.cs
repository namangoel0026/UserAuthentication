using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
using UserManagement.Helpers;
using UserManagement.Services;
using Azure.Core;

namespace UserManagement.Controllers
{
    [Route("api/User")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;

        public UsersController(UserContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
        // GET: api/Users/Authenticate
        //[HttpGet]
        //public async Task<ActionResult<AuthenticateResponse>> Authenticate(AuthenticateRequest request)
        //{
        //    return await Authenticate(request);
        //}

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUser(int id)
        {
            var UserModel = await _context.Users.FindAsync(id);

            if (UserModel == null)
            {
                return NotFound();
            }

            return UserModel;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] UserRequest UserModel)
        {
            foreach (UserRoleModel userRole in UserModel.UserRoles)
            {
                bool roleExists = await _context.Roles.AnyAsync(r => r.Id == userRole.RoleId);
                if (!roleExists)
                {
                    throw new Exception("The specified RoleModel does not exist.");
                }
            }
            var userAdd = new UserModel
            {
                Username = UserModel.Name,
                Email = UserModel.Email,
                UserRoles = (ICollection<UserRoleModel>)UserModel.UserRoles,
                Password = EncryptorDecryptor.HashPassword(UserModel.Password)
            };

            _context.Entry(UserModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserModel>> PostUser(UserRequest UserModel)
        {
            foreach (UserRoleModel userRole in UserModel.UserRoles)
            {
                bool roleExists = await _context.Roles.AnyAsync(r => r.Id == userRole.RoleId);
                if (!roleExists)
                {
                    throw new Exception("The specified RoleModel does not exist.");
                }
            }
            var userAdd = new UserModel
            {
                Username = UserModel.Name,
                Email = UserModel.Email,
                UserRoles = (ICollection<UserRoleModel>)UserModel.UserRoles,
                Password = EncryptorDecryptor.HashPassword(UserModel.Password)
            };
            
            _context.Users.Add(userAdd);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUser", new { id = userAdd.Id }, userAdd);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var UserModel = await _context.Users.FindAsync(id);
            if (UserModel == null)
            {
                return NotFound();
            }

            _context.Users.Remove(UserModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
