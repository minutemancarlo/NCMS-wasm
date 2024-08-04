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
            parameters.Add("@EventName", events.EventName);
            parameters.Add("@EventType", events.EventType);
            parameters.Add("@EventStart", events.EventStart);
            parameters.Add("@EventEnd", events.EventEnd);
            parameters.Add("@IsApproved", events.IsApproved);

            // Execute the stored procedure
            return await _dbConnection.ExecuteScalarAsync<int>("AddUpdateEvents", parameters, commandType: CommandType.StoredProcedure);
        }


    }
}
