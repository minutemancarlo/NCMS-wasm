using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class DTRModel : Employee
    {
    
        public string? CutOffDate { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public DateTime? DTRUpdatedOn { get; set; }

        public DateTime? ShiftDate { get; set; }
        
        public string? TimeInFormatted => TimeIn?.ToString("HH:mm tt");
        public string? TimeOutFormatted => TimeOut?.ToString("HH:mm tt");
    }

    public class DTRLeave
    {
        public string? IDNumber { get; set; }
        public string? LeaveType { get; set; }
        public DateTime? LeaveStart { get; set; }
        public DateTime? LeaveEnd { get; set; }
    }

    public class GenerateDTRRequest
    {
        public string? EmployeeId { get; set; }
        public string? CutOffDate { get; set; }
        public string? DTRType { get; set; }
    }

    public class GeneratedDTR
    {
        public int? TaskId { get; set; }
        public string? TaskName { get; set; }
        public string? TaskType { get; set; }
        public string? FilePath { get; set; }
        public DTRStatus? Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }

    public enum DTRStatus
    {
        On_Queue,
        Processing,
        Processed,
        Failed
    }


}
