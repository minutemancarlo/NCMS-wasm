using Dapper;
using NCMS_wasm.Shared;
using System.Data;

namespace NCMS_wasm.Server.Repository
{
    public class GuestRepository
    {
        private readonly IDbConnection _dbConnection;
        public GuestRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> AddInquiriesAsync(Inquiries inquiries)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Name", inquiries.Name);
            parameters.Add("@Email", inquiries.Email);
            parameters.Add("@Message", inquiries.Message);


            string query = "INSERT INTO Inquiries (Name,Email,Message,CreatedOn,isRead) VALUES (@Name,@Email,@Message,GETDATE(),0)";

            return await _dbConnection.ExecuteAsync(query, parameters);
        }

    }
}
