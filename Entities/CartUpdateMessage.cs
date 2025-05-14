namespace ScrewItStore.Entities;
public class CartUpdateMessage
{
    public string Action { get; set; }
    public string UserId { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
}
