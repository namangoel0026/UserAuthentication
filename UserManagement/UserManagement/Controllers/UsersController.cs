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
using UserManagement.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace UserManagement.Controllers
{
    [Route("api/User")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IUserService _userService;
        private readonly IStringLocalizer<UsersController> _localizer;
        public UsersController(UserContext context, IUserService userService, IStringLocalizer<UsersController> localizer)
        {
            _context = context;
            _userService = userService;
            _localizer = localizer;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            return Ok(await _userService.GetAll());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _userService.GetById(id);

            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> PutUser(UserRequest UserModel)
        {
            var user = await _userService.UpdateUserAsync(UserModel);
            var response = new ApiResponse<UserDTO>(_localizer["UserUpdated"], (UserDTO)user);
            return Ok(response );
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser([FromBody] UserModel UserModel)
        {
            var saveduser = await _userService.CreateUserWithRolesAsync(UserModel);
            var message = _localizer.GetString("UserCreated");
            var response = new ApiResponse<UserDTO>(_localizer["UserCreated"], (UserDTO)saveduser);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleteduser = await _userService.DeleteUserAsync(id);
            var response = new ApiResponse<bool>(_localizer["UserDeleted"], (bool)deleteduser);
            return Ok(response);
        }
    }
}
