namespace Clothify_Project.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public int CategoryID { get; set; }
        public decimal Price { get; set; }
        public int StockCount { get; set; }
        public decimal AverageRating { get; set; }

        // Added CategoryName for display
        public string CategoryName { get; set; }
    }
}
