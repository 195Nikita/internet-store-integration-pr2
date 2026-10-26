namespace InternetStoreIntegration.Domain;

public sealed record Product(
    string Sku,
    string Name,
    decimal Price,
    int Stock);

public sealed record OrderLine(
    string Sku,
    int Quantity);

public sealed record Order(
    string Id,
    string CustomerName,
    IReadOnlyList<OrderLine> Lines);

public sealed record IntegrationResult(
    bool IsSuccess,
    string Message)
{
    public static IntegrationResult Success(string message) => new(true, message);
    public static IntegrationResult Failure(string message) => new(false, message);
}
