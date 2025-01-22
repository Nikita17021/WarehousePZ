using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Warehouse.Models
{
    public class ProductCategoryViewModel
    {
        public List<Product>? Product { get; set; }
        public SelectList? Category { get; set; }
        public string? SelectedCategory { get; set; }
        public string? SearchString { get; set; }

        // Для пагинации
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
    }
}
        