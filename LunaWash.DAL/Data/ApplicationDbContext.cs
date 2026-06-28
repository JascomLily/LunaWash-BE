using System;
using System.Collections.Generic;
using LunaWash.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace LunaWash.DAL.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }
    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<CustomerProfile> CustomerProfiles { get; set; }



    public virtual DbSet<CustomerVehicle> CustomerVehicles { get; set; }



    public virtual DbSet<MembershipTier> MembershipTiers { get; set; }



    public virtual DbSet<Role> Roles { get; set; }



    public virtual DbSet<ServicePrice> ServicePrices { get; set; }



    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VehicleType> VehicleTypes { get; set; }



    public virtual DbSet<WashService> WashServices { get; set; }

    public virtual DbSet<WashSlot> WashSlots { get; set; }

    public virtual DbSet<PointHistory> PointHistories { get; set; }

    public virtual DbSet<ServiceReview> ServiceReviews { get; set; }

    public virtual DbSet<Equipment> Equipments { get; set; }
    
    public virtual DbSet<MaintenanceTask> MaintenanceTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bookings__3214EC07E730194A");

            entity.HasIndex(e => new { e.BookingDate, e.Status }, "IX_Bookings_Date_Status");

            entity.HasIndex(e => e.PriorityScore, "IX_Bookings_PriorityScore").IsDescending();

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BranchId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.VehicleTypeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.WashSlotId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Branch).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Branches");

            entity.HasOne(d => d.Customer).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Users");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.VehicleTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_VehicleTypes");

            entity.HasOne(d => d.WashSlot).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.WashSlotId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Bookings_WashSlots");


        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Branches__3214EC0718E6E656");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.BranchName).HasMaxLength(150);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<CustomerProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Customer__1788CC4C61E0E536");

            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MembershipTierId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MembershipTier).WithMany(p => p.CustomerProfiles)
                .HasForeignKey(d => d.MembershipTierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerProfiles_MembershipTiers");

            entity.HasOne(d => d.User).WithOne(p => p.CustomerProfile)
                .HasForeignKey<CustomerProfile>(d => d.UserId)
                .HasConstraintName("FK_CustomerProfiles_Users");
        });



        modelBuilder.Entity<CustomerVehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CustomerVehicles");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VehicleTypeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LicensePlate).HasMaxLength(50);
            entity.Property(e => e.VehicleModel).HasMaxLength(100);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerVehicles)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_CustomerVehicles_Users");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.CustomerVehicles)
                .HasForeignKey(d => d.VehicleTypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_CustomerVehicles_VehicleTypes");
        });



        modelBuilder.Entity<MembershipTier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Membersh__3214EC07E290A73D");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PointsMultiplier)
                .HasDefaultValue(1.00m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PriorityLevel).HasDefaultValue(1);
            entity.Property(e => e.TierName).HasMaxLength(50);
        });



        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC076A8FEDEE");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });



        modelBuilder.Entity<ServicePrice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ServiceP__3214EC07F18A3DA8");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ServiceId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VehicleTypeId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Service).WithMany(p => p.ServicePrices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_ServicePrices_WashServices");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.ServicePrices)
                .HasForeignKey(d => d.VehicleTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServicePrices_VehicleTypes");
        });



        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC073C923E0F");

            entity.HasIndex(e => e.PhoneNumber, "IX_Users_PhoneNumber");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105344912FDD5").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Password).HasColumnName("Password").HasMaxLength(250);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.RoleId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleT__3214EC071DB6C937");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });



        modelBuilder.Entity<WashService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WashServ__3214EC07ED7A729A");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ServiceName).HasMaxLength(150);
        });

        modelBuilder.Entity<WashSlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WashSlot__3214EC078CE98FDA");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BranchId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.SlotNumber).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Available");

            entity.HasOne(d => d.Branch).WithMany(p => p.WashSlots)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_WashSlots_Branches");
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Attendances");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BranchId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendances_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
