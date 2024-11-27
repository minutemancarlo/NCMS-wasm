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
                parameters.Add("@CardReference", employeeInfo.CardReference);

            // Execute the stored procedure
            return await _dbConnection.ExecuteScalarAsync<int>("AddOrUpdateEmployee", parameters, commandType: CommandType.StoredProcedure);
           

        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            string query = "SELECT *,Profile as ImageUrl FROM Employee";
            return await _dbConnection.QueryAsync<Employee>(query);
        }

        public async Task<Employee> GetEmployeeInfoAsync(string email)
        {
            var parameters = new DynamicParameters();
            // Adding wildcard characters for partial match using the LIKE operator
            parameters.Add("@Email", "%" + email + "%");

            string query = "SELECT TOP 1 *,Profile as ImageUrl FROM Employee WHERE Email LIKE @Email";

            var employee = await _dbConnection.QueryFirstOrDefaultAsync<Employee>(query, parameters);

            return employee;
        }

        public async Task<DTRModel> GetEmployeeInfoRFIDAsync(string rfid)
        {
            var parameters = new DynamicParameters();
            // Adding wildcard characters for partial match using the LIKE operator
            parameters.Add("@RFID", rfid.Trim());

            string query = "SELECT TOP 1 * FROM Employee a left join DTR b on a.IDNumber = b.EMployeeId where a.cardReference = @RFID order by b.shiftDate desc";

            var employee = await _dbConnection.QueryFirstOrDefaultAsync<DTRModel>(query, parameters);

            return employee;
        }

        public async Task<IEnumerable<DTRModel>> GetEmployeeInfoRFIDAllAsync()
        {
            
            string query = "SELECT *,b.UpdateOn as DTRUpdatedOn FROM Employee a inner join DTR b on a.IDNumber = b.EMployeeId order by b.UpdateOn desc";            

            return await _dbConnection.QueryAsync<DTRModel>(query);
        }



        public async Task<int> BindUserAccountAsync(Employee employee)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@IDNumber", employee.IDNumber);
            parameters.Add("@AuthId", employee.Auth0_Id);

            string query = "UPDATE Employee SET Auth0_Id = @AuthId WHERE IDNumber = @IDNumber";
            
            return await _dbConnection.ExecuteAsync(query, parameters);
        }

        public async Task<int> ManageDTRAsync(DTRModel dtr)
        {            
            var parameters = new DynamicParameters();

            parameters.Add("@IDNumber", dtr.IDNumber);
            parameters.Add("@CutOffDate", dtr.CutOffDate);
            parameters.Add("@TimeIn", dtr.TimeIn);
            parameters.Add("@TimeOut", dtr.TimeOut);
            parameters.Add("@ShiftDate", dtr.ShiftDate);
            string query = "ManageDTR";

            return await _dbConnection.ExecuteAsync(query, parameters,commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UnBindUserAccountAsync(string employee)
        {
            var parameters = new DynamicParameters();
            
            parameters.Add("@AuthId", employee);

            string query = "UPDATE Employee SET Auth0_Id = NULL WHERE Auth0_Id = @AuthId";

            return await _dbConnection.ExecuteAsync(query, parameters);
        }

        public async Task<Employee> GetMyInfoAsync(string auth_id)
        {
            var parameters = new DynamicParameters();
            
            parameters.Add("@Id",auth_id);

            string query = "SELECT TOP 1 *,Profile as ImageUrl FROM Employee WHERE Auth0_Id = @Id";

            var employee = await _dbConnection.QueryFirstOrDefaultAsync<Employee>(query, parameters);

            return employee;
        }

        public async Task<Employee> GetEmployeeInfoSingleAsync(string idNumber)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Id", idNumber);

            string query = "SELECT TOP 1 *,Profile as ImageUrl FROM Employee WHERE IDNumber = @Id";

            var employee = await _dbConnection.QueryFirstOrDefaultAsync<Employee>(query, parameters);

            return employee;
        }



    }
}
