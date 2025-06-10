using Microsoft.AspNetCore.SignalR;


namespace A01_FuNewsManagament_API
{
    public class NotificationHub : Hub
    {
        public async Task BroadcastUpdate(string data)
        {
            await Clients.All.SendAsync("ReceiveUpdate", data);
        }
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        // Ghi log khi client ngắt kết nối
        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
