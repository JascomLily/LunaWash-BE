import re

# Update ApplicationDbContext.cs
with open(r'd:\SWP\LunaWash-BE\LunaWash.DAL\Data\ApplicationDbContext.cs', 'r', encoding='utf-8') as f:
    content = f.read()

# Add DbSet<Attendance>
if 'public virtual DbSet<Attendance> Attendances { get; set; }' not in content:
    content = content.replace('public virtual DbSet<Booking> Bookings { get; set; }', 'public virtual DbSet<Booking> Bookings { get; set; }\n    public virtual DbSet<Attendance> Attendances { get; set; }')

# Add modelBuilder for Attendance
if 'modelBuilder.Entity<Attendance>' not in content:
    attendance_mapping = '''
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
'''
    content = content.replace('OnModelCreatingPartial(modelBuilder);', attendance_mapping.strip() + '\n\n        OnModelCreatingPartial(modelBuilder);')

with open(r'd:\SWP\LunaWash-BE\LunaWash.DAL\Data\ApplicationDbContext.cs', 'w', encoding='utf-8') as f:
    f.write(content)

# Update DependencyInjection.cs
with open(r'd:\SWP\LunaWash-BE\LunaWash.BLL\DependencyInjection.cs', 'r', encoding='utf-8') as f:
    di_content = f.read()

if 'IEmployeeService' not in di_content:
    di_content = di_content.replace('services.AddScoped<IAuthService, AuthService>();', 'services.AddScoped<IAuthService, AuthService>();\n            services.AddScoped<IEmployeeService, EmployeeService>();')

with open(r'd:\SWP\LunaWash-BE\LunaWash.BLL\DependencyInjection.cs', 'w', encoding='utf-8') as f:
    f.write(di_content)

print("Updated DbContext and DI!")
