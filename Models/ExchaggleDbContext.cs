using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Exchaggle.Models
{
    public class ExchaggleDbContext : DbContext
    {
        public DbSet<Account> Account { get; set; }
        public DbSet<AccountDetail> AccountDetail { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Subcategory> Subcategory { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<ItemDetail> ItemDetail { get; set; }
        public DbSet<Wishlist> Wishlist { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Offer> Offer { get; set; }
        public DbSet<OfferDetail> OfferDetail { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<Image> Image { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }
    }
}