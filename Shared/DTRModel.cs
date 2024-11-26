using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class DTRModel
    {
        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeProfile { get; set; } = "user.png";
        public string? CutOffDate { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public DateTime? UpdatedOn { get; set; }
        
        public string TimeInFormatted => TimeIn?.ToString("HH:mm tt");
        public string TimeOutFormatted => TimeOut?.ToString("HH:mm tt");
    }
}
