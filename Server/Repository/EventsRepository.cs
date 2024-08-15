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

        public async Task<int> AddEventAsync(Events events)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@EventId", events.EventId);
            parameters.Add("@EventName", events.EventName);
            parameters.Add("@EventSubType", events.SubType);
            parameters.Add("@EventType", events.EventType);
            parameters.Add("@EventStart", events.EventStart);
            parameters.Add("@EventEnd", events.EventEnd);
            parameters.Add("@IsApproved", events.IsApproved);

            // Execute the stored procedure
            return await _dbConnection.ExecuteScalarAsync<int>("AddUpdateEvent", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Events>> GetAllEventsAsync()
        {
            string query = "SELECT EventId ,EventName ,EventSubType ,EventType ,EventStart ,EventEnd , IsApproved FROM Events where IsApproved=1";
            return await _dbConnection.QueryAsync<Events>(query);
        }


    }
}
