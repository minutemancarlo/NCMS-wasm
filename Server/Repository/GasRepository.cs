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

       

        public async Task<string> InsertTransactionAsync(GasModel transaction)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@TransactionType", transaction.TransactionType);
            parameters.Add("@CardTransactionId", transaction.CardTransactionId);
            parameters.Add("@IsVoid", transaction.IsVoid);
            parameters.Add("@IsCard", transaction.IsCard);
            parameters.Add("@Discounts", transaction.Discounts);
            parameters.Add("@VAT", transaction.VAT);
            parameters.Add("@Total", transaction.Total);
            parameters.Add("@CashReceived", transaction.CashReceived);
            parameters.Add("@Change", transaction.Change);
            parameters.Add("@LoyaltyCardId", transaction.LoyaltyCardId);

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
