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
        private readonly ILogger<UserManagementController> _logger;
        public UserManagementController(IManagementApiClient managementApiClient, ILogger<UserManagementController> logger)
        {
            _managementApiClient = managementApiClient;
            _logger = logger;
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
                _logger.LogInformation("Users List Retrieved.");
                return Ok(userslist);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception Occured: {ex.Message}");
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
                _logger.LogInformation("Roles List Retrieved.");
                return Ok(rolesList);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception Occured: {ex.Message}");
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }

        [HttpPost("GetUserRole")]
        public async Task<ActionResult<string>> GetUsersRole([FromBody] string userid)
        {
            try
            {
                var userRole = await GetUserRole(userid);
                _logger.LogInformation("Users Role Retrieved.");
                return Ok(userRole);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception Occured: {ex.Message}");
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
                _logger.LogInformation("User Role Assigned.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception Occured: {ex.Message}");
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserInfo user)
        {
            try
            {
                var data = new UserCreateRequest
                {
                    Email = user.Email,
                    FirstName = user.GivenName,
                    LastName = user.FamilyName,
                    Password = user.Password,
                    EmailVerified = user.EmailVerified,
                    Blocked = user.Blocked,
                    VerifyEmail = user.VerifyEmail,
                    Connection=user.Connection
                };
                User createResult = await _managementApiClient.Users.CreateAsync(data);

                //Default Role if not set is Guest
                var currentData = new AssignRolesRequest
                {
                    Roles = new string[] { string.IsNullOrEmpty(user.RoleId) ? "rol_Auk4u6P2AUid2gwT" : user.RoleId }
                };
                var result = _managementApiClient.Users.AssignRolesAsync(createResult.UserId, currentData);
                _logger.LogInformation("User Added.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception Occured: {ex.Message}");
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
