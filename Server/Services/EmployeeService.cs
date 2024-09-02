using System.Data;

namespace NCMS_wasm.Server.Services
{
    public class EmployeeService
    {
        public DataTable GetEmployees()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Name");
            dt.Columns.Add("Department");
            dt.Columns.Add("Designation");
            dt.Columns.Add("Salary");

            dt.Rows.Add(1, "John", "IT", "Manager", 5000);
            dt.Rows.Add(2, "Smith", "HR", "Executive", 3000);
            dt.Rows.Add(3, "Mark", "IT", "Developer", 4000);
            dt.Rows.Add(4, "Mary", "HR", "Manager", 5000);
            dt.Rows.Add(5, "Sara", "IT", "Developer", 4000);
            dt.Rows.Add(6, "David", "IT", "Developer", 4000);
            dt.Rows.Add(7, "Peter", "HR", "Executive", 3000);
            dt.Rows.Add(8, "Julie", "HR", "Manager", 5000);
            dt.Rows.Add(9, "Tom", "IT", "Developer", 4000);
            dt.Rows.Add(10, "Jerry", "IT", "Developer", 4000);

            return dt;
        }
    }
}
