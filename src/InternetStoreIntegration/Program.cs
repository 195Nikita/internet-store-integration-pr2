using System.Text.Json;
using InternetStoreIntegration.Adapters;
using InternetStoreIntegration.Application;
using InternetStoreIntegration.Domain;

var repositoryRoot = FindRepositoryRoot(AppContext.BaseDirectory);
var inventoryPath = Path.Combine(repositoryRoot, "samples", "inventory.json");
var orderPath = Path.Combine(repositoryRoot, "samples", "order-webhook.json");
var ordersDatabasePath = Path.Combine(repositoryRoot, ".runtime", "orders.jsonl");

var service = new IntegrationService(
    new JsonInventorySource(inventoryPath),
    new ConsoleMarketplaceGateway(),
    new JsonLinesOrderRepository(ordersDatabasePath));

var inventory = await service.SynchronizeInventoryAsync();

await using var orderStream = File.OpenRead(orderPath);
var order = await JsonSerializer.DeserializeAsync<Order>(
    orderStream,
    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

if (order is null)
{
    throw new InvalidDataException("Не удалось прочитать webhook заказа.");
}

var result = await service.AcceptOrderAsync(order, inventory);
Console.WriteLine(result.Message);
Environment.ExitCode = result.IsSuccess ? 0 : 1;

static string FindRepositoryRoot(string startDirectory)
{
    var current = new DirectoryInfo(startDirectory);

    while (current is not null)
    {
        if (Directory.Exists(Path.Combine(current.FullName, "samples")))
        {
            return current.FullName;
        }

        current = current.Parent;
    }

    throw new DirectoryNotFoundException("Не найден корень репозитория с каталогом samples.");
}
