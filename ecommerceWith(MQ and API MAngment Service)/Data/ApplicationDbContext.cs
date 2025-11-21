using System.Text.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Models;
using MultiTenancy.Models.AuthModels;
using MultiTenancy.Models.CheckOutModels;

namespace MultiTenancy.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public string TenantId { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<ProductModel> Products { get; set; }
    public DbSet<BrandModel> Brands { get; set; }
    public DbSet<CategoryModel> Categories { get; set; }
    public DbSet<WishListModel> WishLists { get; set; }
    public DbSet<AddressModel> Addresses { get; set; }
    public DbSet<CartModel> Carts { get; set; }
    public DbSet<CartItemModel> CartItemes { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Items> Items { get; set; }
    
}