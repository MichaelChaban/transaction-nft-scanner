using System.Text.Json;
using TransactionNftScanner.Constants;
using TransactionNftScanner.Contracts;
using TransactionNftScanner.Extensions;

namespace TransactionNftScanner.Services;

public class NftTransactionScannerService(HttpClient httpClient)
{
    public async Task ScanAndSaveNftsAsync(string transactionHash)
    {
        var utxosTransaction = await GetUtxosTransactionAsync(transactionHash);
        if (utxosTransaction is null)
        {
            return;
        }

        var uniqueUnits = utxosTransaction.GetUniqueUnits();
        if (uniqueUnits is null || !uniqueUnits.Any())
        {
            Console.WriteLine("There are no units found with this transaction hash.");
            return;
        }

        var uniqueAssets = await GetUniqueAssetsAsync(uniqueUnits);

        await SaveAssetsAsync(transactionHash, uniqueAssets);
    }

    private async Task<UTXOsTransaction?> GetUtxosTransactionAsync(string transactionHash)
    {
        var utxosTransactionResponse = await httpClient.GetAsync(string.Format(BlockfrostConstants.UTXOsRequestUrl, transactionHash));
        if (!utxosTransactionResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("There are no transaction found with this transaction hash.");
            return null;
        }

        var utxosTransactionContent = await utxosTransactionResponse.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UTXOsTransaction>(utxosTransactionContent,
            options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    private async Task<List<OnchainMetadata>> GetUniqueAssetsAsync(IEnumerable<UTXOsTransactionAmountItem> uniqueUnits)
    {
        var uniqueAssets = new List<OnchainMetadata>();
        foreach (var unit in uniqueUnits)
        {
            var specificAsset = await GetSpecificAssetAsync(unit.Unit);
            if (specificAsset?.Onchain_Metadata is not null)
            {
                uniqueAssets.Add(new OnchainMetadata(
                    Name: specificAsset.Onchain_Metadata.Name,
                    Image: specificAsset.Onchain_Metadata.Image["ipfs://".Length..]));
            }
        }
        return uniqueAssets;
    }

    private async Task<SpecificAsset?> GetSpecificAssetAsync(string unit)
    {
        var specificAssetResponse = await httpClient.GetAsync(string.Format(BlockfrostConstants.SpecificAssetUrl, unit));
        if (!specificAssetResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var specificAssetContent = await specificAssetResponse.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SpecificAsset>(specificAssetContent,
            options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    private async Task SaveAssetsAsync(string transactionHash, List<OnchainMetadata> uniqueAssets)
    {
        Directory.CreateDirectory($"{BlockfrostConstants.TransactionsDirectory}/{transactionHash}");

        foreach (var asset in uniqueAssets)
        {
            var imageIpfsResponse = await httpClient.GetAsync(string.Format(CloudfareConstants.CloudfareIpfsUrl, asset.Image));
            if (!imageIpfsResponse.IsSuccessStatusCode)
            {
                continue;
            }
            string filePath = Path.Combine($"{BlockfrostConstants.TransactionsDirectory}/{transactionHash}", $"{asset.Name}.png");
            using var imageStream = await imageIpfsResponse.Content.ReadAsStreamAsync();
            using FileStream fileStream = File.Create(filePath);
            await imageStream.CopyToAsync(fileStream);
        }
    }
}
