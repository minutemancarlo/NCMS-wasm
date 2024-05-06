using Auth0.ManagementApi;
using Auth0.ManagementApi.Clients;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Client.Pages.Administrator;
using NCMS_wasm.Shared;

namespace NCMS_wasm.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class UserManagementController : ControllerBase
    {

        private readonly IManagementApiClient _managementApiClient;

        public UserManagementController(IManagementApiClient managementApiClient)
        {
            _managementApiClient = managementApiClient;
        }

        [HttpGet("UsersList")]
        //[Authorize(Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<UserDto>>> UsersList()
        {
            try
            {
                var users = await _managementApiClient.Users.GetAllAsync(new GetUsersRequest(), new PaginationInfo());

                List<UserDto> userslist = users.Select(x => new UserDto
                {
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Blocked = x.Blocked ?? false,
                    User_Id = x.UserId,
                    Verified = x.EmailVerified,
                    Last_Login = x.LastLogin,
                    Provider = x.UserId.ToLower().Contains("auth0") ? "Email-Password Auth" :
                               x.UserId.ToLower().Contains("facebook") ? "Facebook" :
                               x.UserId.ToLower().Contains("google-oauth2") ? "Google" :
                               "Unknown Provider",
                    Picture = x.Picture
                }).ToList();
                return Ok(userslist);
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occured: {ex.Message}");
            }

        }

        [HttpGet("GetAllRoles")]
        public async Task<ActionResult<List<RolesProperty>>> GetAllRoles()
        {
            try
            {
                var roles = await _managementApiClient.Roles.GetAllAsync(new GetRolesRequest());
                List<RolesProperty> rolesList = roles.Select(x => new RolesProperty
                {
                    id = x.Id,
                    name = x.Name,
                    description = x.Description
                }).ToList();
                return Ok(rolesList);

            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }

        [HttpPost("GetUserRole")]
        public async Task<ActionResult<string>> GetUsersRole([FromBody] string userid)
        {
            try
            {
                var userRole = await GetUserRole(userid);
                return Ok(userRole);
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }

        [HttpPost("SetUserRole")]
        public async Task<ActionResult<string>> SetUserRole([FromBody] SetUserRole user)
        {
            try
            {
                var currentUserRole = await GetUserRole(user.UserId);
                if (!string.IsNullOrEmpty(currentUserRole))
                {
                    // Initialize the AssignRolesRequest object with the roles to be removed
                    var oldData = new AssignRolesRequest
                    {
                        Roles = new string[] { currentUserRole }
                    };

                    // Call the RemoveRolesAsync method with the user ID and the AssignRolesRequest object
                    await _managementApiClient.Users.RemoveRolesAsync(user.UserId, oldData);
                }
                var currentData = new AssignRolesRequest
                {
                    Roles = new string[] { user.RoleId }
                };
                var result = _managementApiClient.Users.AssignRolesAsync(user.UserId, currentData);                
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }

        private async Task<string> GetUserRole(string userid)
        {
            var result = _managementApiClient.Users.GetRolesAsync(userid);
            var userRole = result.Result.FirstOrDefault()?.Id;
            return userRole;
        }

    }
}
