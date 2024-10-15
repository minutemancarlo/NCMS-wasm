using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    public async Task NotifyUpdate(string message)
    {
        await Clients.All.SendAsync("ReceiveUpdate", message);
    }
}
