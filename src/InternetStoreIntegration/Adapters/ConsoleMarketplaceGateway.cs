using InternetStoreIntegration.Domain;
using InternetStoreIntegration.Ports;

namespace InternetStoreIntegration.Adapters;

public sealed class ConsoleMarketplaceGateway : IMarketplaceGateway
{
    public Task PublishStockAsync(
        IReadOnlyList<Product> products,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[HTTPS/JSON] В маркетплейс передано позиций: {products.Count}");
        return Task.CompletedTask;
    }
}
