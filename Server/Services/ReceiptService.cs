using NCMS_wasm.Server.Repository;
using NCMS_wasm.Shared;
using System.Data;

namespace NCMS_wasm.Server.Services
{
    public class ReceiptService
    {
        private readonly GasRepository _gasRepository;
        public ReceiptService(GasRepository gasRepository)
        {
            _gasRepository = gasRepository;
        }

        public async Task<DataTable> GetReceiptTransactionAsync(string invoiceNo)
        {
            // Create a new DataTable
            DataTable dt = new DataTable();

            // Add columns based on GasModel properties
            dt.Columns.Add("TransactionId", typeof(string));
            dt.Columns.Add("TransactionType", typeof(string));
            dt.Columns.Add("CardTransactionId", typeof(string));
            dt.Columns.Add("IsVoid", typeof(bool));
            dt.Columns.Add("IsCard", typeof(bool));
            dt.Columns.Add("Points", typeof(decimal));
            dt.Columns.Add("Discounts", typeof(decimal));
            dt.Columns.Add("VAT", typeof(decimal));
            dt.Columns.Add("Total", typeof(decimal));
            dt.Columns.Add("DiscountedTotal", typeof(decimal));
            dt.Columns.Add("CashReceived", typeof(decimal));
            dt.Columns.Add("Change", typeof(decimal));
            dt.Columns.Add("LoyaltyCardId", typeof(string));
            dt.Columns.Add("InvoiceNo", typeof(string));
            dt.Columns.Add("CreatedBy", typeof(string));
            dt.Columns.Add("CreatedOn", typeof(DateTime));
            dt.Columns.Add("UpdatedBy", typeof(string));
            dt.Columns.Add("UpdatedOn", typeof(DateTime));

            // Add columns for SubTransaction properties
            dt.Columns.Add("SubTransactionId", typeof(int));
            dt.Columns.Add("GasType", typeof(string));
            dt.Columns.Add("Price", typeof(decimal));
            dt.Columns.Add("Value", typeof(decimal));
            dt.Columns.Add("SubTransactionVAT", typeof(decimal));
            dt.Columns.Add("NetAmount", typeof(decimal));
            dt.Columns.Add("Amount", typeof(decimal));
            dt.Columns.Add("SubTotal", typeof(decimal));
            dt.Columns.Add("SubInvoiceNo", typeof(string));

            TransactionRequest request = await _gasRepository.GetTransactionsForReceipt(invoiceNo);



            // Map the GasModel part of the TransactionRequest
            foreach (var subTransaction in request.SubTransactions)
            {
                DataRow row = dt.NewRow();

                // Fill GasModel properties
                row["TransactionId"] = request.Transaction.TransactionId;
                row["TransactionType"] = request.Transaction.TransactionType.ToString();
                row["CardTransactionId"] = request.Transaction.CardTransactionId;
                row["IsVoid"] = request.Transaction.IsVoid;
                row["IsCard"] = request.Transaction.IsCard;
                row["Points"] = request.Transaction.Points;
                row["Discounts"] = request.Transaction.Discounts;
                row["VAT"] = request.Transaction.VAT;
                row["Total"] = request.Transaction.Total;
                row["DiscountedTotal"] = request.Transaction.DiscountedTotal;
                row["CashReceived"] = request.Transaction.CashReceived;
                row["Change"] = request.Transaction.Change;
                row["LoyaltyCardId"] = request.Transaction.LoyaltyCardId;
                row["InvoiceNo"] = request.Transaction.InvoiceNo;
                row["CreatedBy"] = subTransaction.CreatedBy;                
                

                // Fill SubTransaction properties                
                row["GasType"] = subTransaction.GasType.ToString();                
                row["Value"] = subTransaction.Value;
                row["SubTotal"] = subTransaction.SubTotal;                
                // Add the row to the DataTable
                dt.Rows.Add(row);
            }

            return dt;
        }



    }
}
