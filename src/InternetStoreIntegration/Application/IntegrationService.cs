using InternetStoreIntegration.Domain;
using InternetStoreIntegration.Ports;

namespace InternetStoreIntegration.Application;

public sealed class IntegrationService(
    IInventorySource inventorySource,
    IMarketplaceGateway marketplaceGateway,
    IOrderRepository orderRepository)
{
    public async Task<IReadOnlyList<Product>> SynchronizeInventoryAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await inventorySource.LoadAsync(cancellationToken);

        if (products.Any(product =>
                string.IsNullOrWhiteSpace(product.Sku) ||
                product.Price < 0 ||
                product.Stock < 0))
        {
            throw new InvalidDataException("В складской выгрузке обнаружены некорректные данные.");
        }

        await marketplaceGateway.PublishStockAsync(products, cancellationToken);
        return products;
    }

    public async Task<IntegrationResult> AcceptOrderAsync(
        Order order,
        IReadOnlyList<Product> inventory,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(order.Id) || order.Lines.Count == 0)
        {
            return IntegrationResult.Failure("Заказ не содержит номера или товарных позиций.");
        }

        var stockBySku = inventory.ToDictionary(product => product.Sku, product => product.Stock);

        foreach (var line in order.Lines)
        {
            if (line.Quantity <= 0)
            {
                return IntegrationResult.Failure($"Некорректное количество для SKU {line.Sku}.");
            }

            if (!stockBySku.TryGetValue(line.Sku, out var stock) || stock < line.Quantity)
            {
                return IntegrationResult.Failure($"Недостаточно остатка для SKU {line.Sku}.");
            }
        }

        await orderRepository.SaveAsync(order, cancellationToken);
        return IntegrationResult.Success($"Заказ {order.Id} принят и передан в учетную систему.");
    }
}
