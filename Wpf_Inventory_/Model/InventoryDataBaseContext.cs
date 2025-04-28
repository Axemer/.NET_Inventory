using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wpf_Inventory_.Model
{
    public partial class InventoryDataBaseContext : DbContext
    {
        public InventoryDataBaseContext()
        {
        }

        public InventoryDataBaseContext(DbContextOptions<InventoryDataBaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<DeviceWorkplace> DeviceWorkplace { get; set; }
        public virtual DbSet<Deviceparts> Deviceparts { get; set; }
        public virtual DbSet<DevicepartsDevice> DevicepartsDevice { get; set; }
        public virtual DbSet<Devicetype> Devicetype { get; set; }
        public new virtual DbSet<Model> Model { get; set; }
        public virtual DbSet<Office> Office { get; set; }
        public virtual DbSet<Workplace> Workplace { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#pragma warning disable CS1030 // Директива #warning
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Inventory Data Base;Username=postgres;Password=1911;");
#pragma warning restore CS1030 // Директива #warning
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("device");

                entity.Property(e => e.DeviceId).HasColumnName("device_id");

                //// Условно настраиваем автоинкремент только для SQLite
                if (Database.IsSqlite())
                {
                    entity.Property(e => e.DeviceId)
                        .ValueGeneratedOnAdd(); // AUTOINCREMENT в SQLite ибо почему-то не работает через запросы
                }
                else
                {
                    entity.Property(e => e.DeviceId)
                        .ValueGeneratedNever(); // PostgreSQL управляет сам ID так что не делаем так
                }

                //entity.Property(e => e.DeviceId)
                //    .HasColumnName("device_id")
                //    .ValueGeneratedNever();     ////  старый сгенерированный код 

                entity.Property(e => e.Dateofcommissioning)
                    .HasColumnName("dateofcommissioning")
                    .HasColumnType("date");

                entity.Property(e => e.Devicename)
                    .HasColumnName("devicename")
                    .HasMaxLength(255);

                entity.Property(e => e.DevicetypeId).HasColumnName("devicetype_id");

                entity.Property(e => e.Exception).HasColumnName("exception");

                entity.Property(e => e.Inventorynumber)
                    .HasColumnName("inventorynumber")
                    .HasMaxLength(16);

                entity.Property(e => e.IpAddress)
                    .HasColumnName("ip_address")
                    .HasMaxLength(15);

                entity.Property(e => e.ModelId).HasColumnName("model_id");

                entity.Property(e => e.Note).HasColumnName("note");

                entity.Property(e => e.OfficeId).HasColumnName("office_id");

                entity.Property(e => e.Serialnumber)
                    .HasColumnName("serialnumber")
                    .HasMaxLength(16);

                entity.HasOne(d => d.Devicetype)
                    .WithMany(p => p.Device)
                    .HasForeignKey(d => d.DevicetypeId)
                    .HasConstraintName("fk_device_devicetype");

                entity.HasOne(d => d.Model)
                    .WithMany(p => p.Device)
                    .HasForeignKey(d => d.ModelId)
                    .HasConstraintName("fk_device_model");

                entity.HasOne(d => d.Office)
                    .WithMany(p => p.Device)
                    .HasForeignKey(d => d.OfficeId)
                    .HasConstraintName("fk_device_office");
            });

            modelBuilder.Entity<DeviceWorkplace>(entity =>
            {
                entity.ToTable("device_workplace");

                if (Database.IsSqlite())
                {
                    entity.Property(e => e.DeviceWorkplaceId)
                        .HasColumnName("device_workplace_id")
                        .ValueGeneratedOnAdd(); // для SQLite — AUTOINCREMENT
                }
                else
                {
                    entity.Property(e => e.DeviceWorkplaceId)
                        .HasColumnName("device_workplace_id")
                        .ValueGeneratedNever(); // если ты контролируешь ID сам
                }

                //entity.Property(e => e.DeviceWorkplaceId)
                //    .HasColumnName("device_workplace_id")
                //    .ValueGeneratedNever();

                entity.Property(e => e.DeviceId).HasColumnName("device_id");

                entity.Property(e => e.WorkplaceId).HasColumnName("workplace_id");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.DeviceWorkplace)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_deviceworkplace_device");

                entity.HasOne(d => d.Workplace)
                    .WithMany(p => p.DeviceWorkplace)
                    .HasForeignKey(d => d.WorkplaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_deviceworkplace_workplace");
            });

            modelBuilder.Entity<Deviceparts>(entity =>
            {
                entity.ToTable("deviceparts");

                if (Database.IsSqlite())
                {
                    entity.Property(e => e.DevicepartsId)
                        .HasColumnName("deviceparts_id")
                        .ValueGeneratedOnAdd(); // для SQLite — AUTOINCREMENT
                }
                else
                {
                    entity.Property(e => e.DevicepartsId)
                        .HasColumnName("deviceparts_id")
                        .ValueGeneratedNever(); // если ты контролируешь ID сам
                }

                //entity.Property(e => e.DevicepartsId)
                //    .HasColumnName("deviceparts_id")
                //    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<DevicepartsDevice>(entity =>
            {
                entity.HasKey(e => new { e.DeviceId, e.DevicepartsId })
                    .HasName("deviceparts_device_pkey");

                entity.ToTable("deviceparts_device");

                entity.Property(e => e.DeviceId).HasColumnName("device_id");

                entity.Property(e => e.DevicepartsId).HasColumnName("deviceparts_id");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.DevicepartsDevice)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_devicepartsdevice_device");

                entity.HasOne(d => d.Deviceparts)
                    .WithMany(p => p.DevicepartsDevice)
                    .HasForeignKey(d => d.DevicepartsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_devicepartsdevice_deviceparts");
            });

            modelBuilder.Entity<Devicetype>(entity =>
            {
                entity.ToTable("devicetype");

                if (Database.IsSqlite())
                {
                    entity.Property(e => e.DevicetypeId)
                        .HasColumnName("devicetype_id")
                        .ValueGeneratedOnAdd(); // для SQLite — AUTOINCREMENT
                }
                else
                {
                    entity.Property(e => e.DevicetypeId)
                        .HasColumnName("devicetype_id")
                        .ValueGeneratedNever(); // если ты контролируешь ID сам
                }

                //entity.Property(e => e.DevicetypeId)
                //    .HasColumnName("devicetype_id")
                //    .ValueGeneratedNever();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Model>(entity =>
            {
                entity.ToTable("model");

                if (Database.IsSqlite())
                {
                    entity.Property(e => e.ModelId)
                        .HasColumnName("model_id")
                        .ValueGeneratedOnAdd(); // для SQLite — AUTOINCREMENT
                }
                else
                {
                    entity.Property(e => e.ModelId)
                        .HasColumnName("model_id")
                        .ValueGeneratedNever(); // если ты контролируешь ID сам
                }

                //entity.Property(e => e.ModelId)
                //    .HasColumnName("model_id")
                //    .ValueGeneratedNever();

                entity.Property(e => e.Model1)
                    .IsRequired()
                    .HasColumnName("model")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Office>(entity =>
            {
                entity.ToTable("office");

                if (Database.IsSqlite())
                {
                    entity.Property(e => e.OfficeId)
                        .HasColumnName("office_id")
                        .ValueGeneratedOnAdd(); // для SQLite — AUTOINCREMENT
                }
                else
                {
                    entity.Property(e => e.OfficeId)
                        .HasColumnName("office_id")
                        .ValueGeneratedNever(); // если ты контролируешь ID сам
                }

                entity.Property(e => e.Block)
                    .HasColumnName("block")
                    .HasColumnType("char");

                entity.Property(e => e.Department)
                    .HasColumnName("department")
                    .HasMaxLength(255);

                entity.Property(e => e.Officenum)
                    .IsRequired()
                    .HasColumnName("officenum")
                    .HasMaxLength(10);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Workplace>(entity =>
            {
                entity.ToTable("workplace");

                entity.Property(e => e.WorkplaceId)
                    .HasColumnName("workplace_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.WorkplaceNote)
                    .HasColumnName("workplace_note")
                    .HasMaxLength(255);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
