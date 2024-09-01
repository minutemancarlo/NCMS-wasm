using Dapper;
using NCMS_wasm.Shared;
using System.Data;

namespace NCMS_wasm.Server.Repository
{
    public class EmployeeRepository
    {
        private readonly IDbConnection _dbConnection;
        public EmployeeRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> AddUpdateEmployeeAsync(Employee employeeInfo)
        {

            
                var parameters = new DynamicParameters();
                parameters.Add("@IDNumber", employeeInfo.IDNumber);
                parameters.Add("@Name", employeeInfo.Name);
                parameters.Add("@Address", employeeInfo.Address);
                parameters.Add("@Phone", employeeInfo.Phone);
                parameters.Add("@SSS", employeeInfo.SSS);
                parameters.Add("@PagIbig", employeeInfo.PagIbig);
                parameters.Add("@PHIC", employeeInfo.PHIC);
                parameters.Add("@Profile", employeeInfo.Profile);
                parameters.Add("@DateOfBirth", employeeInfo.DateOfBirth);
                parameters.Add("@PlaceOfBirth", employeeInfo.PlaceOfBirth);
                parameters.Add("@Gender", employeeInfo.Gender);
                parameters.Add("@CivilStatus", employeeInfo.CivilStatus);
                parameters.Add("@Email", employeeInfo.Email);
                parameters.Add("@Position", employeeInfo.Position);
                parameters.Add("@Department", (int)employeeInfo.Department);  // Cast enum to int
                parameters.Add("@EmploymentStatus", (int)employeeInfo.EmploymentStatus);  // Cast enum to int
                parameters.Add("@DateHired", employeeInfo.DateHired);
                parameters.Add("@DateResigned", employeeInfo.DateResigned);
                parameters.Add("@Salary", employeeInfo.Salary);
                parameters.Add("@EmergencyContactName", employeeInfo.EmergencyContactName);
                parameters.Add("@EmergencyContactRelationship", employeeInfo.EmergencyContactRelationship);
                parameters.Add("@EmergencyContactPhone", employeeInfo.EmergencyContactPhone);
                parameters.Add("@EmergencyContactAddress", employeeInfo.EmergencyContactAddress);
                parameters.Add("@BeneficiaryName", employeeInfo.BeneficiaryName);
                parameters.Add("@BeneficiaryRelationship", employeeInfo.BeneficiaryRelationship);
                parameters.Add("@BeneficiaryContactInfo", employeeInfo.BeneficiaryContactInfo);
                parameters.Add("@CreatedBy", employeeInfo.CreatedBy);
                parameters.Add("@CreatedOn", employeeInfo.CreatedOn);
                parameters.Add("@UpdatedBy", employeeInfo.UpdatedBy);
                parameters.Add("@UpdatedOn", employeeInfo.UpdatedOn);


                // Execute the stored procedure
                return await _dbConnection.ExecuteScalarAsync<int>("AddOrUpdateEmployee", parameters, commandType: CommandType.StoredProcedure);
           

        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            string query = "SELECT * FROM Employee";
            return await _dbConnection.QueryAsync<Employee>(query);
        }

        public async Task<Employee> GetEmployeeInfoAsync(string email)
        {
            var parameters = new DynamicParameters();
            // Adding wildcard characters for partial match using the LIKE operator
            parameters.Add("@Email", "%" + email + "%");

            string query = "SELECT TOP 1 * FROM Employee WHERE Email LIKE @Email";

            var employee = await _dbConnection.QueryFirstOrDefaultAsync<Employee>(query, parameters);

            return employee;
        }

        public async Task<int> BindUserAccountAsync(Employee employee)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@IDNumber", employee.IDNumber);
            parameters.Add("@AuthId", employee.Auth0_Id);

            string query = "UPDATE Employee SET Auth0_Id = @AuthId WHERE IDNumber = @IDNumber";
            
            return await _dbConnection.ExecuteAsync(query, parameters);
        }


    }
}
