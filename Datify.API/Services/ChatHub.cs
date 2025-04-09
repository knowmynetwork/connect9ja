using Datify.API.Contracts;
using Datify.API.Data;
using Datify.Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace Datify.API.Services;

public class ChatHub : Hub
{
    
    public ChatHub(IChat chatService)
    {
        this.chatService = chatService;
    }
    
    private readonly IChat chatService;
    
    public async Task SendMessage(string userId, string senderId, MessageDto message)
    {
        await chatService.SendMessage(userId, senderId, message);

        await Clients.All.SendAsync("ReceiveMessage", senderId, message);
    }

    public async Task<List<MessageDto>> GetMessages(string userId, string senderId)
    {
        return await chatService.GetMessages(userId, senderId);
    }   
}