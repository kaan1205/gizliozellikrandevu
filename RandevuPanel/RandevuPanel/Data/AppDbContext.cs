using Microsoft.EntityFrameworkCore;
using RandevuPanel.Models;

namespace RandevuPanel.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<ServiceNote> ServiceNotes => Set<ServiceNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            e.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            e.Property(u => u.Email).HasMaxLength(200).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Appointment>(e =>
        {
            e.HasIndex(a => a.AppointmentDate);
            e.Property(a => a.VehicleBrand).HasMaxLength(100).IsRequired();
            e.Property(a => a.VehicleModel).HasMaxLength(100).IsRequired();
            e.Property(a => a.CustomerName).HasMaxLength(200);
            e.Property(a => a.CustomerPhone).HasMaxLength(20).IsRequired();
            e.Property(a => a.VinNumber).HasMaxLength(17);

            e.HasOne(a => a.CreatedByUser)
             .WithMany(u => u.Appointments)
             .HasForeignKey(a => a.CreatedByUserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ServiceNote>(e =>
        {
            e.Property(n => n.VehicleBrand).HasMaxLength(100);
            e.Property(n => n.VehicleModel).HasMaxLength(100);
            e.Property(n => n.PhotoPath).HasMaxLength(500);
            e.Property(n => n.TotalCost).HasColumnType("decimal(18,2)");

            e.HasOne(n => n.CreatedByUser)
             .WithMany(u => u.ServiceNotes)
             .HasForeignKey(n => n.CreatedByUserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Lead>(e =>
        {
            e.Property(l => l.CustomerName).HasMaxLength(200).IsRequired();
            e.Property(l => l.VehicleBrand).HasMaxLength(100);
            e.Property(l => l.VehicleModel).HasMaxLength(100);

            e.HasOne(l => l.CreatedByUser)
             .WithMany(u => u.Leads)
             .HasForeignKey(l => l.CreatedByUserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
