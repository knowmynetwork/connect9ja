using Datify.Shared.Models;

namespace Datify.API.Data;

public class Message
{
    public string? SenderId { get; set; }
    public string? ReceiverId { get; set; }
    public Information Data { get; set; } = new Information();
    public DateTime Date { get;} = DateTime.Now;
}
