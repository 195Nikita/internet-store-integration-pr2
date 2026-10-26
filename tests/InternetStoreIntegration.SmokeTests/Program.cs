using InternetStoreIntegration.Application;
using InternetStoreIntegration.Domain;
using InternetStoreIntegration.Ports;

var inventory = new[]
{
    new Product("SKU-001", "Клавиатура", 3490m, 10),
    new Product("SKU-002", "Мышь", 1990m, 2)
};

var repository = new InMemoryOrderRepository();
var service = new IntegrationService(
    new InMemoryInventorySource(inventory),
    new NoOpMarketplaceGateway(),
    repository);

var synchronized = await service.SynchronizeInventoryAsync();
Assert(synchronized.Count == 2, "Должны синхронизироваться две позиции.");

var accepted = await service.AcceptOrderAsync(
    new Order("ORDER-TEST-1", "Тестовый покупатель", [new OrderLine("SKU-001", 2)]),
    synchronized);
Assert(accepted.IsSuccess, "Заказ с достаточным остатком должен быть принят.");
Assert(repository.SavedOrders.Count == 1, "Принятый заказ должен быть сохранен.");

var rejected = await service.AcceptOrderAsync(
    new Order("ORDER-TEST-2", "Тестовый покупатель", [new OrderLine("SKU-002", 3)]),
    synchronized);
Assert(!rejected.IsSuccess, "Заказ сверх остатка должен быть отклонен.");

Console.WriteLine("Все smoke-тесты успешно пройдены.");

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

file sealed class InMemoryInventorySource(IReadOnlyList<Product> products) : IInventorySource
{
    public Task<IReadOnlyList<Product>> LoadAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(products);
}

file sealed class NoOpMarketplaceGateway : IMarketplaceGateway
{
    public Task PublishStockAsync(
        IReadOnlyList<Product> products,
        CancellationToken cancellationToken = default) => Task.CompletedTask;
}

file sealed class InMemoryOrderRepository : IOrderRepository
{
    public List<Order> SavedOrders { get; } = [];

    public Task SaveAsync(Order order, CancellationToken cancellationToken = default)
    {
        SavedOrders.Add(order);
        return Task.CompletedTask;
    }
}
