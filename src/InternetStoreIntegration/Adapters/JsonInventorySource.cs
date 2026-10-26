using System.Text.Json;
using InternetStoreIntegration.Domain;
using InternetStoreIntegration.Ports;

namespace InternetStoreIntegration.Adapters;

public sealed class JsonInventorySource(string filePath) : IInventorySource
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IReadOnlyList<Product>> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(filePath);
        var products = await JsonSerializer.DeserializeAsync<List<Product>>(
            stream,
            JsonOptions,
            cancellationToken);

        return products ?? [];
    }
}
