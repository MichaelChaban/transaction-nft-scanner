namespace TransactionNftScanner.Contracts;

public record UTXOsTransactionOutput(
    IEnumerable<UTXOsTransactionAmountItem> Amount);