using Datify.API.Data;
using Datify.Shared.Models;

namespace Datify.API.Contracts;

public interface IChat
{
    Task SendMessage(string userId, string senderId, MessageDto message);
    
    Task<List<MessageDto>> GetMessages(string userId, string senderId);
    
    Task DeleteMessages(string userId, string senderId, string messageId);
}