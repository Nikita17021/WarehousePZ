using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    public class ProductsController : Controller
    {
        private readonly WarehouseContext _context;

        public ProductsController(WarehouseContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString, string selectedCategory, string sortOrder, int pageNumber = 1, int pageSize = 10)
        {
            // Установим порядок сортировки
            ViewData["NameSortOrder"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["QuantitySortOrder"] = sortOrder == "quantity" ? "quantity_desc" : "quantity";

            var viewModel = new ProductCategoryViewModel
            {
                Category = new SelectList(await _context.Category.ToListAsync(), "Id", "Name", selectedCategory),
                SearchString = searchString,
                SelectedCategory = selectedCategory
            };

            var products = from p in _context.Product.Include(p => p.Category)
                           select p;

            // Фильтрация по имени
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            // Фильтрация по категории
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                products = products.Where(p => p.CategoryId == int.Parse(selectedCategory));
            }

            // Сортировка
            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                case "quantity":
                    products = products.OrderBy(p => p.Quantity);
                    break;
                case "quantity_desc":
                    products = products.OrderByDescending(p => p.Quantity);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }

            // Пагинация
            var totalCount = await products.CountAsync(); // Общее количество элементов
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize); // Общее количество страниц

            // Получаем текущую страницу
            var productsOnPage = await products.Skip((pageNumber - 1) * pageSize)
                                               .Take(pageSize)
                                               .AsNoTracking()
                                               .ToListAsync();

            // Передаем информацию о товарах и пагинации в ViewModel
            viewModel.Product = productsOnPage;
            viewModel.CurrentPage = pageNumber;
            viewModel.TotalPages = totalPages;
            viewModel.PageSize = pageSize;

            return View(viewModel);
        }







        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Name");
            return View();
        }
        public async Task<IActionResult> Statistics()
        {
            // Pobierz dane do statystyki
            var totalProducts = await _context.Product.CountAsync();
            var totalQuantity = await _context.Product.SumAsync(p => p.Quantity);
            var totalValue = await _context.Product.SumAsync(p => p.Quantity * p.Price);

            // Zbuduj model statystyki
            var statistics = new ProductStatistics
            {
                TotalProducts = totalProducts,
                TotalQuantity = totalQuantity,
                TotalValue = totalValue
            };

            return View(statistics);
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Quantity,Price,CategoryId")] Product product)
        {
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Name", product.CategoryId);
            return View(product);
        }
       


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Quantity,Price,CategoryId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }
            ModelState.Remove("Category");


            if (ModelState.IsValid)
            {
                try
                {
                    // Пытаемся обновить данные
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Product updated successfully: {product.Id}, {product.Name}, {product.Quantity}, {product.Price}, {product.CategoryId}");

                    // Если обновление прошло успешно
                    TempData["SuccessMessage"] = $"Product with ID {product.Id} was successfully updated!";
                }
                catch (DbUpdateConcurrencyException dbEx)
                {
                    // Ошибка, связанная с параллельным обновлением данных
                    Console.WriteLine($"Concurrency error: {dbEx.Message}");
                    TempData["ErrorMessage"] = "A concurrency error occurred while trying to update the product. Please try again.";
                    return RedirectToAction(nameof(Edit), new { id });
                }
                catch (Exception ex)
                {
                    // Ловим другие ошибки, такие как проблемы с базой данных или валидацией
                    Console.WriteLine($"Error: {ex.Message}");
                    TempData["ErrorMessage"] = "An error occurred while trying to update the product. Please try again later.";
                    return RedirectToAction(nameof(Edit), new { id });
                }
                return RedirectToAction(nameof(Index));
            }
            // В случае ошибки валидации
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Name", product.CategoryId);
            return View(product);
        }
       

       


        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
