namespace TransactionNftScanner.Contracts;

public record UTXOsTransaction(
    IEnumerable<UTXOsTransactionOutput> Inputs,
    IEnumerable<UTXOsTransactionOutput> Outputs);
