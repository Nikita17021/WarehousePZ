namespace Warehouse.Models
{
    public class OutOfStockProductViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public int CurrentQuantity { get; set; }
        public int QuantityToAdd { get; set; }

        public int QuantityToSell { get; set; } // Для продажи
    }

}
