using Housemaid.api.Models;
using Microsoft.EntityFrameworkCore;

namespace Housemaid.api.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{

    public DbSet<User> Users => Set<User>();
    public DbSet<Apartment> Apartments => Set<Apartment>();
    public DbSet<Billing> Billings => Set<Billing>();
    public DbSet<Housing> Housings => Set<Housing>();
    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.UserId);

            entity.Property(u => u.Email).IsRequired().HasMaxLength(30);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.UserName).IsRequired().HasMaxLength(30);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("now()");

            entity.Ignore(u => u.Id);
            // entity.Ignore(u => u.LockoutEnd);
            // entity.Ignore(u => u.LockoutEnabled);
            // entity.Ignore(u => u.AccessFailedCount);
            // entity.Ignore(u => u.EmailConfirmed);
            // entity.Ignore(u => u.ConcurrencyStamp);
            // entity.Ignore(u => u.TwoFactorEnabled);


        });

        modelBuilder.Entity<Housing>(entity =>
        {
            entity.ToTable("housings");
            entity.HasKey(h => h.HouseId);
            entity.HasIndex(h => h.OwnerId);
            entity.HasOne(u => u.Owner).WithMany(u => u.Housings).HasForeignKey(h => h.OwnerId);
            entity.HasMany(h => h.Tenants).WithOne(t => t.House).HasForeignKey(h => h.UserId);

            entity.Property(h => h.HouseName).HasMaxLength(100).IsRequired();
            entity.Property(h => h.Address).HasMaxLength(50).IsRequired();
            entity.Property(h => h.City).HasMaxLength(15).IsRequired();
        });

        modelBuilder.Entity<Apartment>(entity =>
        {
            entity.ToTable("apartments");

            entity.HasKey(a => a.ApartmentId);

            entity.HasOne(a => a.House).WithMany(a => a.Apartments).HasForeignKey(a => a.HouseId);
            entity.HasOne(a => a.Tenant).WithOne(t => t.Apartment).HasForeignKey<Apartment>(a => a.TenantId);

            entity.Property(a => a.UnitNo).IsRequired().HasMaxLength(30);
            entity.Property(a => a.Status).IsRequired().HasConversion<string>();
            entity.Property(a => a.UnitNo).IsRequired().HasMaxLength(30);
            entity.Property(a => a.Details).HasMaxLength(200);

        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("tenants");
            entity.HasKey(t => t.UserId);

            entity.HasOne(t => t.User).WithOne(u => u.Tenant).HasForeignKey<Tenant>(t => t.UserId);
        });

        modelBuilder.Entity<Billing>(entity =>
        {
            entity.ToTable("billings");
            entity.HasKey(b => b.BillId);

            entity.HasOne(b => b.Apartment).WithMany(a => a.Billings).HasForeignKey(b => b.ApartmentId);
            entity.HasOne(b => b.Tenant).WithMany(t => t.Billings).HasForeignKey(b => b.TenantId);
            entity.Property(b => b.PaymentStat).HasConversion<string>();

        });

    }


}
