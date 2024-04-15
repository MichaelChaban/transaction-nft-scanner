using TransactionNftScanner.Constants;
using TransactionNftScanner.Services;

namespace TransactionNftScanner;

public class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Transaction NFT Scanner", ConsoleColor.Green);
        Console.WriteLine("Provide a transaction hash:");
        string? transactionHash;
        while (string.IsNullOrEmpty(transactionHash = Console.ReadLine()))
        {
            Console.WriteLine("A Transaction NFT Hash was not provided.");
        }

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add(BlockfrostConstants.ProjectIdKey, BlockfrostConstants.ProjectIdValue);

        var scanner = new NftTransactionScannerService(httpClient);
        await scanner.ScanAndSaveNftsAsync(transactionHash);
        Console.WriteLine($"\n[{DateTime.Now}]: NFT Transaction Scanning completed successfully.\n");
    }
}
