using Dapper;
using NCMS_wasm.Shared;
using System.Data;

namespace NCMS_wasm.Server.Repository
{
    public class GasRepository
    {
        private readonly IDbConnection _dbConnection;
        public GasRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<GasPrice>> GetAllGasPricesAsync()
        {                                          
            return await _dbConnection.QueryAsync<GasPrice>("Select * from GasPrices", null, commandType: CommandType.Text);
        }

        public async Task<IEnumerable<GasModel>> GetTransactions()
        {
            return await _dbConnection.QueryAsync<GasModel>("Select * from Transactions Order By TransactionId Desc", null, commandType: CommandType.Text);
        }


        public async Task<LoyaltyCardInfo> GetCardInfoAsync(string cardReference)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CardType", RFIDType.Loyalty);
            parameters.Add("@CardReference", cardReference);
            parameters.Add("@SelectType", 1);

            string sp = "SelectAllCards";
            return await _dbConnection.QuerySingleOrDefaultAsync<LoyaltyCardInfo>(sp, parameters, commandType: CommandType.StoredProcedure);
        }



        public async Task<string> InsertTransactionAsync(GasModel transaction)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@TransactionType", transaction.TransactionType);
            parameters.Add("@CardTransactionId", transaction.CardTransactionId);
            parameters.Add("@IsVoid", transaction.IsVoid);
            parameters.Add("@IsCard", transaction.IsCard);
            parameters.Add("@Discounts", transaction.Discounts);
            var discounted = transaction.Total - transaction.Discounts;
            parameters.Add("@DiscountedTotal", discounted);
            parameters.Add("@VAT", transaction.VAT);
            parameters.Add("@Total", transaction.Total);
            parameters.Add("@CashReceived", transaction.CashReceived);
            parameters.Add("@Change", transaction.Change);
            parameters.Add("@LoyaltyCardId", transaction.LoyaltyCardId);
            parameters.Add("@Points", transaction.Points);

            var invoiceNo = await _dbConnection.QuerySingleAsync<string>(
                "InsertTransaction",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return invoiceNo;
        }

        public async Task<int> InsertSubTransactionAsync(SubTransaction transaction)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@GasType", transaction.GasType);
            parameters.Add("@Price", transaction.Price);
            parameters.Add("@Value", transaction.Value);
            parameters.Add("@VAT", transaction.VAT);
            parameters.Add("@NetAmount", transaction.NetAmount);
            parameters.Add("@Amount", transaction.Amount);
            parameters.Add("@SubTotal", transaction.SubTotal);
            parameters.Add("@CreatedBy", transaction.CreatedBy);
            parameters.Add("@InvoiceNo", transaction.InvoiceNo);

            var id = await _dbConnection.QuerySingleAsync<int>(
                "InsertSubTransaction",
                parameters, 
                commandType: CommandType.StoredProcedure
            );
            return id;
            
        }

        public async Task<int> InsertLoyaltyCardInfoAsync(LoyaltyCardInfo request)
        {
            var parameters = new DynamicParameters();
            
            parameters.Add("@FirstName", request.FirstName);
            parameters.Add("@MiddleName", request.MiddleName);
            parameters.Add("@LastName", request.LastName);
            parameters.Add("@Email", request.Email);
            parameters.Add("@PhoneNumber", request.PhoneNumber);
            parameters.Add("@CardReference", request.CardReference);
            parameters.Add("@Type", request.Type);
            parameters.Add("@IsActive", request.IsActive);
            parameters.Add("@Points", request.Points);
            parameters.Add("@CreatedBy", request.CreatedBy);
            
            var id = await _dbConnection.ExecuteScalarAsync<int>(
                "AddLoyaltyCardInfo",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return id;
        }


        public async Task<int> UpdateGasPriceAsync(GasPrice gasPrice)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", gasPrice.ID);
            parameters.Add("@Price", gasPrice.Price);
            parameters.Add("@CapacityRemaining", gasPrice.CapacityRemaining);
            parameters.Add("@UpdatedBy", gasPrice.UpdatedBy);
            var id = await _dbConnection.ExecuteScalarAsync<int>(
              "UpdateGasPrice",
              parameters,
              commandType: CommandType.StoredProcedure
          );
            return id;


        }

    }
}
