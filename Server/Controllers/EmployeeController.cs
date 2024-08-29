using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Server.Repository;
using NCMS_wasm.Shared;

namespace NCMS_wasm.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly EmployeeRepository _employeeRepository;

        public EmployeeController(ILogger<EmployeeController> logger, EmployeeRepository employeeRepository)
        {
            _logger = logger;
            _employeeRepository = employeeRepository;
        }

        [HttpPost("AddUpdateEmployee")]
        public async Task<ActionResult<int>> AddUpdateEmployee(Employee employeeInfo)
        {
            try
            {
                int Id = await _employeeRepository.AddUpdateEmployeeAsync(employeeInfo);
                _logger.LogInformation("Employee added/updated successfully.");
                return Ok(Id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while adding/updating employee: {ex.Message}");
                return BadRequest($"Exception occurred while adding/updating employee: {ex.Message}");
            }
        }


    }
}
