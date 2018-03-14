using ApplicationCore.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using System.IO;

namespace Infrastructure.Data
{

    public class CatalogContextInitialSetup : DbContext
    {
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           optionsBuilder.UseMySQL(getconn());
           
        }

        public DbSet<Basket> Baskets { get; set; }
        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Basket>(ConfigureBasket);
            builder.Entity<CatalogBrand>(ConfigureCatalogBrand);
            builder.Entity<CatalogType>(ConfigureCatalogType);
            builder.Entity<CatalogItem>(ConfigureCatalogItem);
            builder.Entity<Order>(ConfigureOrder);
            builder.Entity<OrderItem>(ConfigureOrderItem);
        }

        private void ConfigureBasket(EntityTypeBuilder<Basket> builder)
        {
            builder.ToTable("baskets");
            var navigation = builder.Metadata.FindNavigation(nameof(Basket.Items));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }

        private void ConfigureCatalogItem(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("catalogitem");

            builder.Property(ci => ci.Id)
               // .ForSqlServerUseSequenceHiLo("catalog_hilo")
                .IsRequired();

            builder.Property(ci => ci.Name)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(ci => ci.Price)
                .IsRequired(true);

            builder.Property(ci => ci.PictureUri)
                .IsRequired(false);

            builder.HasOne(ci => ci.CatalogBrand)
                .WithMany()
                .HasForeignKey(ci => ci.CatalogBrandId);

            builder.HasOne(ci => ci.CatalogType)
                .WithMany()
                .HasForeignKey(ci => ci.CatalogTypeId);
        }

        private void ConfigureCatalogBrand(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable("catalogbrandnew");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
              // .ForSqlServerUseSequenceHiLo("catalog_brand_hilo")
               .IsRequired();

            builder.Property(cb => cb.Brand)
                .IsRequired()
                .HasMaxLength(100);
        }

        private void ConfigureCatalogType(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ToTable("catalogtypenew");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
             //  .ForSqlServerUseSequenceHiLo("catalog_type_hilo")
               .IsRequired();

            builder.Property(cb => cb.Type)
                .IsRequired()
                .HasMaxLength(100);
        }

        private void ConfigureOrder(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");
            var navigation = builder.Metadata.FindNavigation(nameof(Order.OrderItems));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.OwnsOne(o => o.ShipToAddress);
        }

        private void ConfigureOrderItem(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("orderitems");
            builder.OwnsOne(i => i.ItemOrdered);
        }

        public static string getconn()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "connstr.txt");
            string[] list = System.IO.File.ReadAllLines(path);
            string connstr = "";

            foreach (string c in list)
            {
                if (!c.StartsWith("//"))
                {
                    connstr = c;
                }
            }

            return connstr;
        }
    }
}
