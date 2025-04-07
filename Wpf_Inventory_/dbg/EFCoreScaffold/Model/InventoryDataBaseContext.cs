using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EFCoreScaffold.Model;

public partial class InventoryDataBaseContext : DbContext
{
    public InventoryDataBaseContext()
    {
    }

    public InventoryDataBaseContext(DbContextOptions<InventoryDataBaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<DeviceWorkplace> DeviceWorkplaces { get; set; }

    public virtual DbSet<Devicepart> Deviceparts { get; set; }

    public virtual DbSet<Devicetype> Devicetypes { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<Office> Offices { get; set; }

    public virtual DbSet<Workplace> Workplaces { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Inventory Data Base;Username=postgres;Password=1911;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("device_pkey");

            entity.ToTable("device");

            entity.Property(e => e.DeviceId)
                .ValueGeneratedNever()
                .HasColumnName("device_id");
            entity.Property(e => e.Dateofcommissioning).HasColumnName("dateofcommissioning");
            entity.Property(e => e.Devicename)
                .HasMaxLength(255)
                .HasColumnName("devicename");
            entity.Property(e => e.DevicetypeId).HasColumnName("devicetype_id");
            entity.Property(e => e.Exception).HasColumnName("exception");
            entity.Property(e => e.Inventorynumber)
                .HasMaxLength(16)
                .HasColumnName("inventorynumber");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(15)
                .HasColumnName("ip_address");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.OfficeId).HasColumnName("office_id");
            entity.Property(e => e.Serialnumber)
                .HasMaxLength(16)
                .HasColumnName("serialnumber");

            entity.HasOne(d => d.Devicetype).WithMany(p => p.Devices)
                .HasForeignKey(d => d.DevicetypeId)
                .HasConstraintName("fk_device_devicetype");

            entity.HasOne(d => d.Model).WithMany(p => p.Devices)
                .HasForeignKey(d => d.ModelId)
                .HasConstraintName("fk_device_model");

            entity.HasOne(d => d.Office).WithMany(p => p.Devices)
                .HasForeignKey(d => d.OfficeId)
                .HasConstraintName("fk_device_office");

            entity.HasMany(d => d.Deviceparts).WithMany(p => p.Devices)
                .UsingEntity<Dictionary<string, object>>(
                    "DevicepartsDevice",
                    r => r.HasOne<Devicepart>().WithMany()
                        .HasForeignKey("DevicepartsId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_devicepartsdevice_deviceparts"),
                    l => l.HasOne<Device>().WithMany()
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_devicepartsdevice_device"),
                    j =>
                    {
                        j.HasKey("DeviceId", "DevicepartsId").HasName("deviceparts_device_pkey");
                        j.ToTable("deviceparts_device");
                        j.IndexerProperty<int>("DeviceId").HasColumnName("device_id");
                        j.IndexerProperty<int>("DevicepartsId").HasColumnName("deviceparts_id");
                    });
        });

        modelBuilder.Entity<DeviceWorkplace>(entity =>
        {
            entity.HasKey(e => e.DeviceWorkplaceId).HasName("device_workplace_pkey");

            entity.ToTable("device_workplace");

            entity.Property(e => e.DeviceWorkplaceId)
                .ValueGeneratedNever()
                .HasColumnName("device_workplace_id");
            entity.Property(e => e.DeviceId).HasColumnName("device_id");
            entity.Property(e => e.WorkplaceId).HasColumnName("workplace_id");

            entity.HasOne(d => d.Device).WithMany(p => p.DeviceWorkplaces)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_deviceworkplace_device");

            entity.HasOne(d => d.Workplace).WithMany(p => p.DeviceWorkplaces)
                .HasForeignKey(d => d.WorkplaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_deviceworkplace_workplace");
        });

        modelBuilder.Entity<Devicepart>(entity =>
        {
            entity.HasKey(e => e.DevicepartsId).HasName("deviceparts_pkey");

            entity.ToTable("deviceparts");

            entity.Property(e => e.DevicepartsId)
                .ValueGeneratedNever()
                .HasColumnName("deviceparts_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Devicetype>(entity =>
        {
            entity.HasKey(e => e.DevicetypeId).HasName("devicetype_pkey");

            entity.ToTable("devicetype");

            entity.Property(e => e.DevicetypeId)
                .ValueGeneratedNever()
                .HasColumnName("devicetype_id");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.ModelId).HasName("model_pkey");

            entity.ToTable("model");

            entity.Property(e => e.ModelId)
                .ValueGeneratedNever()
                .HasColumnName("model_id");
            entity.Property(e => e.Model1)
                .HasMaxLength(255)
                .HasColumnName("model");
        });

        modelBuilder.Entity<Office>(entity =>
        {
            entity.HasKey(e => e.OfficeId).HasName("office_pkey");

            entity.ToTable("office");

            entity.Property(e => e.OfficeId)
                .ValueGeneratedNever()
                .HasColumnName("office_id");
            entity.Property(e => e.Block)
                .HasColumnType("char")
                .HasColumnName("block");
            entity.Property(e => e.Department)
                .HasMaxLength(255)
                .HasColumnName("department");
            entity.Property(e => e.Officenum)
                .HasMaxLength(10)
                .HasColumnName("officenum");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Workplace>(entity =>
        {
            entity.HasKey(e => e.WorkplaceId).HasName("workplace_pkey");

            entity.ToTable("workplace");

            entity.Property(e => e.WorkplaceId)
                .ValueGeneratedNever()
                .HasColumnName("workplace_id");
            entity.Property(e => e.WorkplaceNote)
                .HasMaxLength(255)
                .HasColumnName("workplace_note");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
