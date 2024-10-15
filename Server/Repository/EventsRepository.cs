using Dapper;
using NCMS_wasm.Shared;
using System.Data;

namespace NCMS_wasm.Server.Repository
{
    public class EventsRepository
    {
        private readonly IDbConnection _dbConnection;
        public EventsRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> AddEventAsync(LeaveRequests events)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@EventId", events.EventId);
            parameters.Add("@EmployeeId", events.EmployeeId);
            parameters.Add("@LeaveType", events.LeaveType);
            parameters.Add("@EventName", events.EventName);
            parameters.Add("@EventSubType", events.EventType == EventsType.Holiday ? events.SubType : "");
            parameters.Add("@EventType", events.EventType);
            parameters.Add("@EventStart", events.EventStart);
            parameters.Add("@EventEnd", events.EventEnd);
            parameters.Add("@IsApproved", events.IsApproved);

            // Execute the stored procedure
            return await _dbConnection.ExecuteScalarAsync<int>("AddUpdateEvent", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteEventAsync(Events events)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@EventId", events.EventId);

            return await _dbConnection.ExecuteScalarAsync<int>("Delete FROM Events where EventId = @EventId ", parameters, commandType: CommandType.Text);
        }

        public async Task<IEnumerable<LeaveRequests>> GetAllEventsAsync(EventFilter filter)
        {
            string query = String.Empty;
            if (filter.IsUser)
            {
                 query = $"Select * from events a left join LeaveRequests b on a.EventId=b.EventId where b.EmployeeId = '{filter.UserId}' ";
            }
            else
            {
                 query = "Select * from events a left join LeaveRequests b on a.EventId=b.EventId;";
            }
            
            return await _dbConnection.QueryAsync<LeaveRequests>(query);
        }

        public async Task<int> ApproveLeaveRequestAsync(LeaveRequests events)
        {
            // Create parameters for the stored procedure
            var parameters = new DynamicParameters();
            parameters.Add("@EventId", events.EventId);
            parameters.Add("@IsApproved", events.IsApproved);
            parameters.Add("@UpdatedBy", events.UpdatedBy);

            // Specify the stored procedure name
            string storedProcedureName = "UpdateLeaveRequest";

            // Execute the stored procedure
            return await _dbConnection.ExecuteAsync(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
        }





    }
}
