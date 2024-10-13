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

namespace RoleManagement.Controllers
{
    [Route("api/RoleModel")]
    [ApiController]
    
    public class RoleController : ControllerBase
    {
        private readonly UserContext _context;

        public RoleController(UserContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleModel>>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }
        // GET: api/Roles/Authenticate
        //[HttpGet]
        //public async Task<ActionResult<AuthenticateResponse>> Authenticate(AuthenticateRequest request)
        //{
        //    return await Authenticate(request);
        //}

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleModel>> GetRole(int id)
        {
            var RoleModel = await _context.Roles.FindAsync(id);

            if (RoleModel == null)
            {
                return NotFound();
            }

            return RoleModel;
        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(RoleModel RoleModel)
        {
            _context.Entry(RoleModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(RoleModel.Id))
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

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoleModel>> PostRole(RoleModel RoleModel)
        {
            _context.Roles.Add(RoleModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetRole", new { id = RoleModel.Id }, RoleModel);
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var RoleModel = await _context.Roles.FindAsync(id);
            if (RoleModel == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(RoleModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}
