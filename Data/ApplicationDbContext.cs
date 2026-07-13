using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using recoverytest3.Models;

namespace recoverytest3.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // The Master Table
        public DbSet<ItemModel> Items { get; set; }

        // The Relational Category Tables
        public DbSet<IdItem> IdItems { get; set; }
        public DbSet<DeviceItem> DeviceItems { get; set; }
        public DbSet<WalletItem> WalletItems { get; set; }
        public DbSet<JewelryItem> JewelryItems { get; set; }
        public DbSet<NotebookItem> NotebookItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // This strictly enforces the Table-Per-Type (TPT) architecture in SQL Server
            modelBuilder.Entity<ItemModel>().ToTable("Items");
            modelBuilder.Entity<IdItem>().ToTable("IdItems");
            modelBuilder.Entity<DeviceItem>().ToTable("DeviceItems");
            modelBuilder.Entity<WalletItem>().ToTable("WalletItems");
            modelBuilder.Entity<JewelryItem>().ToTable("JewelryItems");
            modelBuilder.Entity<NotebookItem>().ToTable("NotebookItems");
        }
    }
}
