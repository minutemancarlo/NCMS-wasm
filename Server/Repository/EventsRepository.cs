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
            var parameters = new DynamicParameters();
            parameters.Add("@EventId", events.EventId);
            parameters.Add("@IsApproved", events.IsApproved);

            // Fetch the updatedBy user name from the database
            string userQuery = $"Select Name from employee where Auth0_id='{events.UpdatedBy}'";
            string? result = await _dbConnection.ExecuteScalarAsync<string>(userQuery);

            // Check if result is null before using it
            if (result != null)
            {
                parameters.Add("@UpdatedBy", result);
            }
            else
            {                
                parameters.Add("@UpdatedBy", DBNull.Value);
            }

            string query = @"
                Update Events 
                    set IsApproved = @IsApproved 
                    where EventId = @EventId;

                Update LeaveRequests 
                    set UpdatedBy = @UpdatedBy, UpdatedOn = GETDATE() 
                    where EventId = @EventId;
                ";

            return await _dbConnection.ExecuteAsync(query, parameters);
        }



    }
}
