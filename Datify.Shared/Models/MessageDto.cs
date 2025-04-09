namespace Datify.Shared.Models;

public class MessageDto
{
    public string? SenderId { get; set; }
    public string? ReceiverId { get; set; }
    public Information Data { get; set; }
}