namespace TransactionNftScanner.Constants;

public static class BlockfrostConstants
{
    public const string ProjectIdKey = "project_id";

    public const string ProjectIdValue = "mainnetRUrPjKhpsagz4aKOCbvfTPHsF0SmwhLc";

    public const string UTXOsRequestUrl = "https://cardano-mainnet.blockfrost.io/api/v0/txs/{0}/utxos";

    public const string SpecificAssetUrl = "https://cardano-mainnet.blockfrost.io/api/v0/assets/{0}";

    public const string TransactionsDirectory = "Transactions";
}
