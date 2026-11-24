using FinalProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Data;

public class AddDbContext(DbContextOptions<AddDbContext> options ):DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); 
        modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(p => p.Name)
                    .HasMaxLength(150);
                
                entity.HasIndex(p => p.Sku)
                    .IsUnique();
                
                entity.Property(p => p.Sku)
                    .HasMaxLength(40);
                
                entity.Property(p => p.Price)
                    .HasColumnType("decimal(10,2)");
                
                entity.ToTable(t => t.HasCheckConstraint("CK_Product_Price_Positive", "\"Price\" > 0"));
                
                entity.Property(p => p.StockQuantity)
                    .HasDefaultValue(0);
                
                entity.ToTable(t => t.HasCheckConstraint("CK_Product_Stock_NonNegative", "\"StockQuantity\" >= 0"));
                
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        );

        modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .HasMaxLength(100);
                entity.HasIndex(e => e.Name)
                    .IsUnique();
                entity.Property(e => e.Description)
                    .HasMaxLength(300);


            }
        );
        
        
    }
    
    
}