using Dapper;
using NCMS_wasm.Shared;
using System.Data;

namespace NCMS_wasm.Server.Repository
{
    public class EmailRepository
    {
        private readonly IDbConnection _dbConnection;
        public EmailRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<EmailModel>> GetQueuedEmailsAsync()
        {
            string query = "Select * from EmailMessages Where EmailStatus=0";
            return await _dbConnection.QueryAsync<EmailModel>(query, commandType: CommandType.Text);
        }

        public async Task<int> MarkEmailAsSentAsync(EmailModel email)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", email.Id);
            parameters.Add("@EmailStatus", EmailStatus.Success);
            string query = "Update EmailMessages Set SentDate = GETDATE(), EmailStatus = @EmailStatus Where Id = @Id";
            return await _dbConnection.ExecuteAsync(query, parameters, commandType: CommandType.Text);
        }

        public async Task<int> MarkEmailAsFailedAsync(EmailModel email)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", email.Id);
            parameters.Add("@EmailStatus", EmailStatus.Failed);
            string query = "Update EmailMessages Set SentDate = GETDATE(), EmailStatus = @EmailStatus Where Id = @Id";
            return await _dbConnection.ExecuteAsync(query, parameters, commandType: CommandType.Text);
        }



    }
}
