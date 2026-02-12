using Microsoft.EntityFrameworkCore;
using SaleManagementSys.Models;

namespace SaleManagementSys.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleDetail> SaleDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Sale entity
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20);
                entity.Property(e => e.Email)
                    .HasMaxLength(200);
                entity.Property(e => e.SaleDate)
                    .IsRequired();
                entity.Property(e => e.TotalAmount)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalProfit)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
            });

            // Configure SaleDetail entity
            modelBuilder.Entity<SaleDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.PurchasePrice)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.SalePrice)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.Quantity)
                    .IsRequired();
                entity.Property(e => e.Description)
                    .HasMaxLength(1000);
            });

            // Configure one-to-many relationship between Sale and SaleDetail
            modelBuilder.Entity<SaleDetail>()
                .HasOne(sd => sd.Sale)
                .WithMany(s => s.SaleDetails)
                .HasForeignKey(sd => sd.SaleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
