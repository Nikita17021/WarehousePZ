using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    public class OutOfStockController : Controller
    {
        private readonly WarehouseContext _context;

        public OutOfStockController(WarehouseContext context)
        {
            _context = context;
        }

        // GET: OutOfStock
        public async Task<IActionResult> Index()
        {
            var outOfStockProducts = await _context.Product
                .Include(p => p.Category)
                .Where(p => p.Quantity == 0)
                .Select(p => new OutOfStockProductViewModel
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    CategoryName = p.Category.Name,
                    CurrentQuantity = p.Quantity,
                    QuantityToAdd = 0 // Default value
                })
                .ToListAsync();

            return View(outOfStockProducts);
        }

        // POST: OutOfStock/OrderProducts
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderProducts(List<OutOfStockProductViewModel> products)
        {
            if (products == null || !products.Any())
            {
                TempData["ErrorMessage"] = "No products to update.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var productVm in products)
            {
                var product = await _context.Product.FindAsync(productVm.ProductId);
                if (product != null && productVm.QuantityToAdd > 0)
                {
                    product.Quantity += productVm.QuantityToAdd;
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Quantities updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: OutOfStock/Sell
        public async Task<IActionResult> Sell()
        {
            var products = await _context.Product
                .Include(p => p.Category)
                .Where(p => p.Quantity > 0)
                .Select(p => new OutOfStockProductViewModel
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    CategoryName = p.Category.Name,
                    CurrentQuantity = p.Quantity,
                    QuantityToAdd = 0, // Default value
                    QuantityToSell = 0 // Default value for selling
                })
                .ToListAsync();

            return View(products);
        }

        // POST: OutOfStock/SellProducts
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SellProducts(List<OutOfStockProductViewModel> products)
        {
            if (products == null || !products.Any())
            {
                TempData["ErrorMessage"] = "No products to update.";
                return RedirectToAction(nameof(Sell));
            }

            foreach (var productVm in products)
            {
                var product = await _context.Product.FindAsync(productVm.ProductId);
                if (product != null && productVm.QuantityToSell > 0)
                {
                    if (product.Quantity >= productVm.QuantityToSell)
                    {
                        product.Quantity -= productVm.QuantityToSell; // Уменьшаем количество
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Not enough stock for product {product.Name}. Only {product.Quantity} available.";
                        return RedirectToAction(nameof(Sell));
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Quantities updated successfully!";
            return RedirectToAction(nameof(Sell));
        }

    }
}
