using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Warehouse.Models;

namespace Warehouse.Data
{
    public class WarehouseContext : DbContext
    {
        public WarehouseContext (DbContextOptions<WarehouseContext> options)
            : base(options)
        {
        }
        public DbSet<Warehouse.Models.Category> Category { get; set; }
        public DbSet<Warehouse.Models.Product> Product { get; set; } = default!;
    }
}
