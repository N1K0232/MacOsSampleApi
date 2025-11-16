using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;
using Microsoft.EntityFrameworkCore;
using MacOsSampleApi.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacOsSampleApi.DataAccessLayer;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<City> Cities { get; set; }

    public DbSet<Person> People { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(b => 
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.Id).ValueGeneratedOnAdd();
            b.Property(c => c.Name).HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<Person>(b => 
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Id).ValueGeneratedOnAdd();

            b.Property(p => p.FirstName).HasMaxLength(255).IsRequired();
            b.Property(p => p.LastName).HasMaxLength(255).IsRequired();

            b.HasOne(p => p.City)
                .WithMany(c => c.People)
                .HasForeignKey(p => p.CityId)
                .IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}