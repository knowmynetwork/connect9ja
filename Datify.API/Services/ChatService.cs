using AutoMapper;
using Datify.API.Contracts;
using Datify.API.Data;
using Datify.Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace Datify.API.Services;

public class ChatService : IChat
{
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;

    public async Task SendMessage(string userId, string senderId, MessageDto message)
    {
        var result = dbContext.Users.Find(userId);
        var result1 = dbContext.Users.Find(senderId);
        if (result == null || result1 == null)
        {
            throw new Exception($"User not found");
        }
        var newMessage = mapper.Map<Message>(message);
        await dbContext.Messages.AddAsync(newMessage);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<MessageDto>> GetMessages(string userId, string senderId)
    {
        var result = await dbContext.Users.FindAsync(userId);
        var result1 = await dbContext.Users.FindAsync(senderId);
        if (result == null || result1 == null)
        {
            throw new Exception($"User not found");
        }
        List<MessageDto> messages = new List<MessageDto>();
        var result2 = dbContext.Messages.Where(u => u.SenderId == senderId)
            .Where(u => u.ReceiverId == userId);
        foreach (var message in result2)
        {
            var dto = mapper.Map<MessageDto>(message);
        }

        return messages;
    }

    public Task DeleteMessages(string userId, string senderId, string messageId)
    {
        throw new NotImplementedException();
    }
}