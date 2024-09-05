using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class Events : BaseModel
    {
        public int EventId { get; set; } = 0;
        public string? EventName { get; set; }
        public EventsType EventType { get; set; } = EventsType.Holiday;
        public DateTime EventStart { get; set; } = DateTime.Today;
        public DateTime EventEnd { get; set; } = DateTime.Today;
        public ApprovalType IsApproved { get; set; } = ApprovalType.ForApproval;
        public string SubType { get; set; } = "Regular Holiday";
    }

    public class EventFilter
    {
        public bool IsUser { get; set; }
        public string? UserId { get; set; }
    }



    public class LeaveRequests : Events
    {        
        public string? EmployeeId { get; set; }
        public LeaveType LeaveType { get; set; }
    }

    public class LeaveReportFilter
    {
        public LeaveType? LeaveType { get; set; } 
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public DateTime? FileDateFrom { get; set; } = null;
        public DateTime? FileDateTo { get; set; } = null;
        public string? EmployeeName { get; set; } 
        public ApprovalType? ApprovalType { get; set; }
    }

        public enum EventsType
    {
        Holiday = 0,
        Leave = 1
    }


    public enum ApprovalType
    {
        ForApproval = 0,
        Approved = 1,
        Rejected = 2
    }

    public enum LeaveType
    {
        Vacation = 1,
        Sick = 2
    }


}
