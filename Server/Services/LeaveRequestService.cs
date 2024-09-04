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
            foreach (var x in events)
            {
                string leaveType = x.LeaveType == LeaveType.Vacation ? "Vacation" : "Sick";
                string status = x.IsApproved == ApprovalType.ForApproval ? "For Approval" : x.IsApproved == ApprovalType.Approved ? "Approved" : "Rejected";                
                dt.Rows.Add(x.EventName.Split(" - ")[1], leaveType, status, x.EventStart, x.EventEnd, x.CreatedOn);
            }

            return dt;
        }
    }
}
