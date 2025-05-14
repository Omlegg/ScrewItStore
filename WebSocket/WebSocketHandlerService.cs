using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ScrewItBackEnd.Data;
using ScrewItBackEnd.Entities;
using ScrewItStore.Entities;

public class WebSocketHandlerService
{
    private readonly IServiceProvider _serviceProvider;

    public WebSocketHandlerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task HandleCartWebSocket(WebSocket webSocket, IServiceProvider services)
    {
        var buffer = new byte[1024];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = JsonSerializer.Deserialize<CartUpdateMessage>(
                        Encoding.UTF8.GetString(buffer, 0, result.Count));

                    using var scope = services.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ScrewItDbContext>();

                    var (success, newAmount) = await ProcessCartUpdate(dbContext, message);

                    if (success)
                    {
                        var response = new CartUpdateResponse
                        {
                            ProductId = message.ProductId,
                            NewAmount = newAmount,
                            NewTotal = newAmount * message.Price
                        };

                        await SendJsonResponse(webSocket, response);
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                }

                Array.Clear(buffer, 0, buffer.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket error: {ex.Message}");
            await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, 
                "Server error", CancellationToken.None);
        }
    }

    private async Task<(bool Success, int NewAmount)> ProcessCartUpdate(
        ScrewItDbContext dbContext, CartUpdateMessage message)
    {
        var cartItem = await dbContext.Carts
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => 
                c.UserId == message.UserId && 
                c.ProductId == message.ProductId);

        try
        {
            switch (message.Action)
            {
                case "add":
                    if (cartItem == null)
                    {
                        dbContext.Carts.Add(new Cart
                        {
                            UserId = message.UserId,
                            ProductId = message.ProductId,
                            AmountInCart = 1
                        });
                        return (true, 1);
                    }
                    cartItem.AmountInCart++;
                    return (true, cartItem.AmountInCart);

                case "remove":
                    if (cartItem == null) return (false, 0);

                    if (cartItem.AmountInCart > 1)
                    {
                        cartItem.AmountInCart--;
                        return (true, cartItem.AmountInCart);
                    }
                    dbContext.Carts.Remove(cartItem);
                    return (true, 0);

                case "delete":
                    if (cartItem != null)
                    {
                        dbContext.Carts.Remove(cartItem);
                        return (true, 0);
                    }
                    return (false, 0);

                default:
                    return (false, 0);
            }

            await dbContext.SaveChangesAsync();
        }
        catch
        {
            return (false, 0);
        }
    }

    private async Task SendJsonResponse(WebSocket webSocket, object response)
    {
        await webSocket.SendAsync(
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response)),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }
}