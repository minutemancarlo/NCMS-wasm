﻿using Auth0.ManagementApi;
using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Server.Repository;
using NCMS_wasm.Shared;
using Nextended.Core.Extensions;

namespace NCMS_wasm.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly EmployeeRepository _employeeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        private readonly IManagementApiClient _managementApiClient;
        public EmployeeController(ILogger<EmployeeController> logger, EmployeeRepository employeeRepository, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, IManagementApiClient managementApiClient)
        {
            _logger = logger;
            _employeeRepository = employeeRepository;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
            _managementApiClient = managementApiClient;
        }

        [HttpPost("AddUpdateEmployee")]
        public async Task<ActionResult<int>> AddUpdateEmployee(Employee employeeInfo)
        {
            try
            {
                if(employeeInfo.Profile != "images/user-alt-solid.svg")
                {

                }


                if(employeeInfo.IDNumber != "0")
                {
                    
                        int Id = await _employeeRepository.AddUpdateEmployeeAsync(employeeInfo);
                        _logger.LogInformation("Employee added/updated successfully.");
                        return Ok(Id);
                   
                }
                else
                {
                    var users = await _managementApiClient.Users.GetUsersByEmailAsync(employeeInfo.Email);
                    if (users.Count == 0)
                    {
                        int Id = await _employeeRepository.AddUpdateEmployeeAsync(employeeInfo);
                        _logger.LogInformation("Employee added/updated successfully.");
                        return Ok(Id);
                    }
                    else
                    {
                        return Conflict("Email Already Exists");
                    }
                }
               
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while adding/updating employee: {ex.Message}");
                return BadRequest($"Exception occurred while adding/updating employee: {ex.Message}");
            }
        }

        [HttpGet("GetEmployees")]
        public async Task<ActionResult<List<Employee>>> GetEmployees()
        {
            try
            {
                var devices = await _employeeRepository.GetAllEmployeesAsync();
                _logger.LogInformation("Employee list retrieved successfully.");
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while retrieving employee list: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving employee list: {ex.Message}");
            }
        }

        [HttpPost("GetEmployeeInfo")]
        public async Task<ActionResult<Employee>> GetEmployeeInfo([FromBody]string email)
        {            
            try
            {
                var employee = await _employeeRepository.GetEmployeeInfoAsync(email);

                if (employee == null)
                {
                    _logger.LogInformation($"Employee with email '{email}' not found.");
                    return NotFound($"Employee with email '{email}' not found.");
                }
                else
                {
                    var userInfo = await _managementApiClient.Users.GetUsersByEmailAsync(employee.Email);
                    if (userInfo.Count > 0)
                    {
                        employee.Auth0_Id = userInfo[0].UserId;
                    }
                    await _employeeRepository.BindUserAccountAsync(employee);
                    _logger.LogInformation($"Employee information retrieved successfully for email '{email}'.");
                    return Ok(employee);
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while retrieving employee info for email '{email}': {ex.Message}");
                return BadRequest($"Exception occurred while retrieving employee info: {ex.Message}");
            }
        }

        [HttpPost("GetMyInfo")]
        public async Task<ActionResult<Employee>> GetMyInfo([FromBody] string auth_id)
        {
            try
            {
                var employee = await _employeeRepository.GetMyInfoAsync(auth_id);

                if (employee == null)
                {
                    _logger.LogInformation($"Employee not bound yet.");
                    return NotFound($"Employee not bound yet.");
                }
                else
                {                   
                    _logger.LogInformation($"Employee information retrieved successfully");
                    return Ok(employee);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while retrieving employee info: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving employee info: {ex.Message}");
            }
        }


        private string SaveImageToDisk(string base64String)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            string serverAddress = $"{request.Scheme}://{request.Host.Value}";
            string filePath = ""; // Define a variable to hold the file path

            // Convert the base64 string back to byte array
            byte[] bytes = Convert.FromBase64String(base64String);

            // Generate a unique filename
            string fileName = Guid.NewGuid().ToString() + ".jpg"; // You can change the extension based on the image type

            // Combine the file path with the file name
            //filePath = Path.Combine("C:\\MenuFlix", fileName); // Modify the path as needed
            filePath = Path.Combine($"{_env.WebRootPath}\\images\\profiles\\", fileName); // Modify the path as needed

            // Write the byte array to the file
            System.IO.File.WriteAllBytes(filePath, bytes);
            var serverFilePath = $"{serverAddress}/images/profiles/{fileName}";
            return serverFilePath;
        }

    }
}
