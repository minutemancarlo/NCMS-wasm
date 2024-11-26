using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class DTRModel
    {
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime? CutOffDate { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
    }
}
