using TransactionNftScanner.Contracts;

namespace TransactionNftScanner.Extensions;

public static class NftTransactionScannerExtensions
{
    public static IEnumerable<UTXOsTransactionAmountItem> GetUniqueUnits(this UTXOsTransaction utxosTransaction)
    {
        return utxosTransaction.Outputs
            .SelectMany(output => output.Amount.Where(amountItem => amountItem.Quantity == "1" && amountItem.Unit != "lovelace"))
            .Concat(utxosTransaction.Inputs.SelectMany(input => input.Amount.Where(amountItem => amountItem.Quantity == "1" && amountItem.Unit != "lovelace")))
            .Distinct();
    }
}
