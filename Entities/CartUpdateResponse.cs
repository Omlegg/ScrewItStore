namespace ScrewItStore.Entities;
    public class CartUpdateResponse
    {
        public int ProductId { get; set; }
        public int NewAmount { get; set; }
        public decimal NewTotal { get; set; }
    }
