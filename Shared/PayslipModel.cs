using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class PayslipModel : BaseModel
    {
        public int PayslipId { get; set; } = 0;
        public string UploadId { get; set; }
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Position { get; set; }
        public DateTime PayrollDate { get; set; } = DateTime.Today;
        public decimal BasicSalary { get; set; } = 0;
        public decimal SSS { get; set; } = 0;
        public decimal PagIbig { get; set; } = 0;
        public decimal PHIC { get; set; } = 0;
        public decimal Tax { get; set; } = 0;        
        public decimal TotalNetPay { get; set; } = 0;
    }
}

