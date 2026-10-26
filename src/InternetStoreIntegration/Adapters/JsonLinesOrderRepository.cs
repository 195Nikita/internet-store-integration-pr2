using System.Text.Json;
using InternetStoreIntegration.Domain;
using InternetStoreIntegration.Ports;

namespace InternetStoreIntegration.Adapters;

public sealed class JsonLinesOrderRepository(string filePath) : IOrderRepository
{
    public async Task SaveAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(order);
        await File.AppendAllTextAsync(filePath, json + Environment.NewLine, cancellationToken);
    }
}
