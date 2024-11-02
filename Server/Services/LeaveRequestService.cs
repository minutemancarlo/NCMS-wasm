using NCMS_wasm.Server.Repository;
using NCMS_wasm.Shared;
using System.Data;
using System.Threading.Tasks;

namespace NCMS_wasm.Server.Services
{
    public class LeaveRequestService
    {
        private readonly EventsRepository _eventsRepository;

        

        public LeaveRequestService(EventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        public async Task<DataTable> GetLeaveRequestsAsync(LeaveReportFilter filter)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("LeaveType");
            dt.Columns.Add("Status");
            dt.Columns.Add("From", typeof(DateTime));
            dt.Columns.Add("To", typeof(DateTime));
            dt.Columns.Add("FiledOn", typeof(DateTime));
            var eventFilter = new EventFilter { IsUser = false };
            var events = await _eventsRepository.GetAllEventsAsync(eventFilter);
            events = events.Where(request => request.EventType == EventsType.Leave)
                        .OrderByDescending(request => request.CreatedOn)
                        .ToList();
            events = FilterLeaveRequests((List<LeaveRequests>)events, filter);
            foreach (var x in events)
            {
                string leaveType = x.LeaveType == LeaveType.Vacation ? "Vacation" : "Sick";
                string status = x.IsApproved == ApprovalType.ForApproval ? "For Approval" : x.IsApproved == ApprovalType.Approved ? "Approved" : "Rejected";                
                dt.Rows.Add(x.EventName.Split(" - ")[1], leaveType, status, x.EventStart, x.EventEnd, x.CreatedOn);
            }

            return dt;
        }

        public List<LeaveRequests> FilterLeaveRequests(List<LeaveRequests> leaveRequests, LeaveReportFilter filter)
        {
            var filteredLeaveRequests = leaveRequests.AsQueryable();

            if (filter.LeaveType.HasValue)
            {
                filteredLeaveRequests = filteredLeaveRequests.Where(lr => lr.LeaveType == filter.LeaveType.Value);
            }

            if (!string.IsNullOrEmpty(filter.EmployeeName))
            {
                filteredLeaveRequests = filteredLeaveRequests.Where(lr => lr.EventName.ToLower().Contains(filter.EmployeeName.ToLower()));
            }

            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                filteredLeaveRequests = filteredLeaveRequests.Where(lr => lr.EventStart >= filter.StartDate.Value && lr.EventEnd <= filter.EndDate.Value);
            }

            if (filter.FileDateFrom.HasValue && filter.FileDateTo.HasValue)
            {
                filteredLeaveRequests = filteredLeaveRequests.Where(lr => lr.CreatedOn >= filter.FileDateFrom.Value && lr.CreatedOn <= filter.FileDateTo.Value);
            }

            if (filter.ApprovalType.HasValue)
            {
                filteredLeaveRequests = filteredLeaveRequests.Where(lr => lr.IsApproved == filter.ApprovalType.Value);
            }

            return filteredLeaveRequests.ToList();
        }

    }
}
