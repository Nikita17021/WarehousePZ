using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new WarehouseContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<WarehouseContext>>()))
            {
                // Проверяем, есть ли уже данные в таблицах Products и Categories
                if (context.Product.Any() || context.Category.Any())
                {
                    return; // База данных уже заполнена
                }

                // Добавление категорий
                var categories = new[]
                {
                    new Category { Name = "Electronics", Description = "Devices and gadgets" },
                    new Category { Name = "Books", Description = "Books and literature" },
                    new Category { Name = "Clothing", Description = "Apparel and accessories" },
                    new Category { Name = "Home Appliances", Description = "Appliances for home use" },
                    new Category { Name = "Sports Equipment", Description = "Equipment for sports and fitness" },
                    new Category { Name = "Toys", Description = "Toys for kids of all ages" },
                    new Category { Name = "Furniture", Description = "Home and office furniture" },
                    new Category { Name = "Groceries", Description = "Everyday food items and supplies" }
                };

                context.Category.AddRange(categories);

                // Добавление продуктов
                var products = new[]
                {
                    // Electronics
                    new Product { Name = "Smartphone", Quantity = 50, Price = 699.99M, Category = categories[0] },
                    new Product { Name = "Laptop", Quantity = 30, Price = 1299.99M, Category = categories[0] },
                    new Product { Name = "Wireless Earbuds", Quantity = 100, Price = 49.99M, Category = categories[0] },
                    new Product { Name = "4K TV", Quantity = 20, Price = 899.99M, Category = categories[0] },

                    // Books
                    new Product { Name = "Science Fiction Novel", Quantity = 100, Price = 15.99M, Category = categories[1] },
                    new Product { Name = "Programming Guide", Quantity = 80, Price = 29.99M, Category = categories[1] },
                    new Product { Name = "History Textbook", Quantity = 60, Price = 19.99M, Category = categories[1] },

                    // Clothing
                    new Product { Name = "Winter Jacket", Quantity = 20, Price = 79.99M, Category = categories[2] },
                    new Product { Name = "Running Shoes", Quantity = 40, Price = 59.99M, Category = categories[2] },
                    new Product { Name = "T-Shirts (Pack of 3)", Quantity = 70, Price = 29.99M, Category = categories[2] },

                    // Home Appliances
                    new Product { Name = "Microwave Oven", Quantity = 15, Price = 129.99M, Category = categories[3] },
                    new Product { Name = "Refrigerator", Quantity = 10, Price = 499.99M, Category = categories[3] },
                    new Product { Name = "Vacuum Cleaner", Quantity = 25, Price = 199.99M, Category = categories[3] },

                    // Sports Equipment
                    new Product { Name = "Yoga Mat", Quantity = 50, Price = 19.99M, Category = categories[4] },
                    new Product { Name = "Dumbbells (Set of 2)", Quantity = 30, Price = 39.99M, Category = categories[4] },
                    new Product { Name = "Basketball", Quantity = 60, Price = 14.99M, Category = categories[4] },

                    // Toys
                    new Product { Name = "Lego Set", Quantity = 40, Price = 49.99M, Category = categories[5] },
                    new Product { Name = "Toy Car", Quantity = 80, Price = 9.99M, Category = categories[5] },
                    new Product { Name = "Dollhouse", Quantity = 20, Price = 89.99M, Category = categories[5] },

                    // Furniture
                    new Product { Name = "Office Desk", Quantity = 10, Price = 149.99M, Category = categories[6] },
                    new Product { Name = "Dining Table", Quantity = 5, Price = 299.99M, Category = categories[6] },
                    new Product { Name = "Bookshelf", Quantity = 15, Price = 99.99M, Category = categories[6] },

                    // Groceries
                    new Product { Name = "Pasta (500g)", Quantity = 100, Price = 1.99M, Category = categories[7] },
                    new Product { Name = "Olive Oil (1L)", Quantity = 50, Price = 8.99M, Category = categories[7] },
                    new Product { Name = "Breakfast Cereal", Quantity = 70, Price = 4.99M, Category = categories[7] }
                };

                context.Product.AddRange(products);

                // Сохранение данных в базу
                context.SaveChanges();
            }
        }
    }
}
