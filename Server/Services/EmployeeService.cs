using NCMS_wasm.Server.Repository;
using NCMS_wasm.Shared;
using System.Data;

namespace NCMS_wasm.Server.Services
{
    public class EmployeeService
    {
        private readonly EmployeeRepository _employeeRepository;
        private readonly PayslipRepository _payslipRepository;
        public EmployeeService(EmployeeRepository employeeRepository, PayslipRepository payslipRepository)
        {
            _employeeRepository = employeeRepository;
            _payslipRepository = payslipRepository;
        }

        public async Task<DataTable> GetEmployeeInfoAsync(string idNumber)
        {

            DataTable dt = new DataTable();
            
            dt.Columns.Add("IDNumber", typeof(string));
            dt.Columns.Add("Auth0_Id", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Address", typeof(string));
            dt.Columns.Add("Phone", typeof(string));
            dt.Columns.Add("SSS", typeof(string));
            dt.Columns.Add("PagIbig", typeof(string));
            dt.Columns.Add("PHIC", typeof(string));
            dt.Columns.Add("Profile", typeof(string));
            dt.Columns.Add("DateOfBirth", typeof(DateTime));
            dt.Columns.Add("PlaceOfBirth", typeof(string));
            dt.Columns.Add("Gender", typeof(string));
            dt.Columns.Add("CivilStatus", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("Position", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("EmploymentStatus", typeof(string));
            dt.Columns.Add("DateHired", typeof(DateTime));
            dt.Columns.Add("DateResigned", typeof(DateTime));
            dt.Columns.Add("Salary", typeof(decimal));
            dt.Columns.Add("EmergencyContactName", typeof(string));
            dt.Columns.Add("EmergencyContactRelationship", typeof(string));
            dt.Columns.Add("EmergencyContactPhone", typeof(string));
            dt.Columns.Add("EmergencyContactAddress", typeof(string));
            dt.Columns.Add("BeneficiaryName", typeof(string));
            dt.Columns.Add("BeneficiaryRelationship", typeof(string));
            dt.Columns.Add("BeneficiaryContactInfo", typeof(string));


            var employeeInfo = await _employeeRepository.GetEmployeeInfoSingleAsync(idNumber);


            if (employeeInfo != null)
            {

                DataRow row = dt.NewRow();
                
                row["IDNumber"] = employeeInfo.IDNumber ?? string.Empty;
                row["Auth0_Id"] = employeeInfo.Auth0_Id ?? string.Empty;
                row["Name"] = employeeInfo.Name;
                row["Address"] = employeeInfo.Address;
                row["Phone"] = employeeInfo.Phone;
                row["SSS"] = employeeInfo.SSS;
                row["PagIbig"] = employeeInfo.PagIbig;
                row["PHIC"] = employeeInfo.PHIC;
                row["Profile"] = employeeInfo.Profile;
                row["DateOfBirth"] = employeeInfo.DateOfBirth;
                row["PlaceOfBirth"] = employeeInfo.PlaceOfBirth;
                row["Gender"] = employeeInfo.Gender;
                row["CivilStatus"] = employeeInfo.CivilStatus;
                row["Email"] = employeeInfo.Email;
                row["Position"] = employeeInfo.Position;
                row["Department"] = employeeInfo.Department.ToString();
                row["EmploymentStatus"] = employeeInfo.EmploymentStatus.ToString();
                row["DateHired"] = employeeInfo.DateHired;
                row["DateResigned"] =  DBNull.Value;
                row["Salary"] = employeeInfo.Salary;
                row["EmergencyContactName"] = employeeInfo.EmergencyContactName;
                row["EmergencyContactRelationship"] = employeeInfo.EmergencyContactRelationship;
                row["EmergencyContactPhone"] = employeeInfo.EmergencyContactPhone;
                row["EmergencyContactAddress"] = employeeInfo.EmergencyContactAddress;
                row["BeneficiaryName"] = employeeInfo.BeneficiaryName;
                row["BeneficiaryRelationship"] = employeeInfo.BeneficiaryRelationship;
                row["BeneficiaryContactInfo"] = employeeInfo.BeneficiaryContactInfo;

                
                dt.Rows.Add(row);
            }

            return dt;
        }

        public async Task<DataTable> GetMyPayslipAsync(int idNumber)
        {
            DataTable dt = new DataTable();
            var payslipInfo = await _payslipRepository.GetMyPayslipInfoAsync(idNumber);

            dt.Columns.Add("PayslipId", typeof(int));            
            dt.Columns.Add("EmployeeId", typeof(string));
            dt.Columns.Add("EmployeeName", typeof(string));
            dt.Columns.Add("Position", typeof(string));
            dt.Columns.Add("PayrollDate", typeof(DateTime));
            dt.Columns.Add("BasicSalary", typeof(decimal));
            dt.Columns.Add("SSS", typeof(decimal));
            dt.Columns.Add("PagIbig", typeof(decimal));
            dt.Columns.Add("PHIC", typeof(decimal));
            dt.Columns.Add("Tax", typeof(decimal));
            dt.Columns.Add("TotalDeductions", typeof(decimal));
            dt.Columns.Add("TotalNetPay", typeof(decimal));
            dt.Columns.Add("TotalSalary", typeof(decimal));

            if (payslipInfo != null)
            {                
                DataRow row = dt.NewRow();
                row["PayslipId"] = payslipInfo.PayslipId;     
                row["EmployeeId"] = payslipInfo.EmployeeId ?? string.Empty;
                row["EmployeeName"] = payslipInfo.EmployeeName ?? string.Empty;
                row["Position"] = payslipInfo.Position ?? string.Empty;
                row["PayrollDate"] = payslipInfo.PayrollDate;
                row["BasicSalary"] = payslipInfo.BasicSalary;
                row["SSS"] = payslipInfo.SSS;
                row["PagIbig"] = payslipInfo.PagIbig;
                row["PHIC"] = payslipInfo.PHIC;
                row["Tax"] = payslipInfo.Tax;
                row["TotalDeductions"] = payslipInfo.SSS + payslipInfo.PagIbig + payslipInfo.PHIC + payslipInfo.Tax;
                row["TotalNetPay"] = payslipInfo.TotalNetPay;
                row["TotalSalary"] = payslipInfo.TotalNetPay - (payslipInfo.SSS + payslipInfo.PagIbig + payslipInfo.PHIC + payslipInfo.Tax);

                dt.Rows.Add(row);
            }

            return dt;
        }



    }
}
