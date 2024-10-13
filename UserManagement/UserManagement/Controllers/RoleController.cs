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
using UserManagement.DTO;
using Microsoft.Extensions.Localization;
using UserManagement.Controllers;
using System.Data;

namespace RoleManagement.Controllers
{
    [Route("api/Role")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IRoleService _roleService;
        private readonly IStringLocalizer<RoleController> _localizer;

        public RoleController(UserContext context, IRoleService roleService, IStringLocalizer<RoleController> localizer)
        {
            _context = context;
            _roleService = roleService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleModel>>> GetRoles()
        {
            return Ok(await _roleService.GetAll());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleModel>> GetRole(int id)
        {
            return Ok(await _roleService.GetById(id));
        }

        [HttpPut]
        public async Task<IActionResult> PutRole(RoleModel RoleModel)
        {
            var role=await _roleService.UpdateRoleAsync(RoleModel);
            var response = new ApiResponse<RoleModel>(_localizer["RoleUpdated"], (RoleModel)role);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<RoleModel>> PostRole(RoleModel RoleModel)
        {
            var role = await _roleService.CreateRoleAsync(RoleModel);
            var response = new ApiResponse<RoleModel>(_localizer["RoleCreated"], (RoleModel)role);
            return Ok(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role=await _roleService.DeleteRoleAsync(id);
            var response = new ApiResponse<bool>(_localizer["RoleDeleted"], (bool)role);
            return Ok(response);

        }
    }
}
