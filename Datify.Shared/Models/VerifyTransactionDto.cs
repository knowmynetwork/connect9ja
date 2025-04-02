namespace Datify.Shared.Models;

public class VerifyTransactionDto
{
    public string TransactionId { get; set; } = default!;
    public string UserId { get; set; }
    public string TrxRef { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = default!;
    public long RelatedEntityId
    {
        get; set;
    }
    public string? RelatedEntity { get; set; }
}