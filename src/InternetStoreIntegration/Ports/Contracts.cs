using InternetStoreIntegration.Domain;

namespace InternetStoreIntegration.Ports;

public interface IInventorySource
{
    Task<IReadOnlyList<Product>> LoadAsync(CancellationToken cancellationToken = default);
}

public interface IMarketplaceGateway
{
    Task PublishStockAsync(
        IReadOnlyList<Product> products,
        CancellationToken cancellationToken = default);
}

public interface IOrderRepository
{
    Task SaveAsync(Order order, CancellationToken cancellationToken = default);
}
