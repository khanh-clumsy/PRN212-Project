using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TrafficViolationFeedbackSystem.Models;

public partial class TrafficViolationFeedbackSystemContext : DbContext
{
    public TrafficViolationFeedbackSystemContext()
    {
    }

    public TrafficViolationFeedbackSystemContext(DbContextOptions<TrafficViolationFeedbackSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appeal> Appeals { get; set; }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Fine> Fines { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<Violation> Violations { get; set; }

    public virtual DbSet<ViolationType> ViolationTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json", true, true);
        var configuration = builder.Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("Default"));
    }
 //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
 //       => optionsBuilder.UseSqlServer("Server= QUY;uid=sa;password=123;database=TrafficViolationFeedbackSystem;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appeal>(entity =>
        {
            entity.HasKey(e => e.AppealId).HasName("PK__Appeals__BB684E10BFFA020F");

            entity.Property(e => e.AppealId).HasColumnName("AppealID");
            entity.Property(e => e.Result).HasMaxLength(50);
            entity.Property(e => e.SubmitDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ViolationId).HasColumnName("ViolationID");
            entity.Property(e => e.ViolatorId).HasColumnName("ViolatorID");

            entity.HasOne(d => d.Violation).WithMany(p => p.Appeals)
                .HasForeignKey(d => d.ViolationId)
                .HasConstraintName("FK__Appeals__Violati__571DF1D5");

            entity.HasOne(d => d.Violator).WithMany(p => p.Appeals)
                .HasForeignKey(d => d.ViolatorId)
                .HasConstraintName("FK__Appeals__Violato__5812160E");
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("PK__Attachme__442C64DE2D2D1A86");

            entity.Property(e => e.AttachmentId).HasColumnName("AttachmentID");
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.FileType).HasMaxLength(10);
            entity.Property(e => e.ReportId).HasColumnName("ReportID");

            entity.HasOne(d => d.Report).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.ReportId)
                .HasConstraintName("FK__Attachmen__Repor__48CFD27E");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__AuditLog__5E5499A8516969F6");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AuditLogs__UserI__60A75C0F");
        });

        modelBuilder.Entity<Fine>(entity =>
        {
            entity.HasKey(e => e.FineId).HasName("PK__Fines__9D4A9BCC123C2B3B");

            entity.Property(e => e.FineId).HasColumnName("FineID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsConfirmed).HasDefaultValue(false);
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.ViolationId).HasColumnName("ViolationID");

            entity.HasOne(d => d.Violation).WithMany(p => p.Fines)
                .HasForeignKey(d => d.ViolationId)
                .HasConstraintName("FK__Fines__Violation__52593CB8");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32CD285FFA");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.PlateNumber).HasMaxLength(15);
            entity.Property(e => e.SentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__5BE2A6F2");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Reports__D5BD48E5CBB1CC98");

            entity.Property(e => e.ReportId).HasColumnName("ReportID");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.PlateNumber).HasMaxLength(15);
            entity.Property(e => e.ReportDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReporterId).HasColumnName("ReporterID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.ViolationTypeId).HasColumnName("ViolationTypeID");

            entity.HasOne(d => d.ProcessedByNavigation).WithMany(p => p.ReportProcessedByNavigations)
                .HasForeignKey(d => d.ProcessedBy)
                .HasConstraintName("FK__Reports__Process__45F365D3");

            entity.HasOne(d => d.Reporter).WithMany(p => p.ReportReporters)
                .HasForeignKey(d => d.ReporterId)
                .HasConstraintName("FK__Reports__Reporte__412EB0B6");

            entity.HasOne(d => d.ViolationType).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ViolationTypeId)
                .HasConstraintName("FK__Reports__Violati__4222D4EF");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC32620D06");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105344A38F598").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicles__476B54B27B16475E");

            entity.HasIndex(e => e.PlateNumber, "UQ__Vehicles__03692624A5369CE0").IsUnique();

            entity.Property(e => e.VehicleId).HasColumnName("VehicleID");
            entity.Property(e => e.Brand).HasMaxLength(50);
            entity.Property(e => e.Model).HasMaxLength(50);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.PlateNumber).HasMaxLength(15);

            entity.HasOne(d => d.Owner).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK__Vehicles__OwnerI__3C69FB99");
        });

        modelBuilder.Entity<Violation>(entity =>
        {
            entity.HasKey(e => e.ViolationId).HasName("PK__Violatio__18B6DC28AAA185EA");

            entity.Property(e => e.ViolationId).HasColumnName("ViolationID");
            entity.Property(e => e.FineAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FineDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaidStatus).HasDefaultValue(false);
            entity.Property(e => e.ReportId).HasColumnName("ReportID");
            entity.Property(e => e.ViolatorId).HasColumnName("ViolatorID");

            entity.HasOne(d => d.Report).WithMany(p => p.Violations)
                .HasForeignKey(d => d.ReportId)
                .HasConstraintName("FK__Violation__Repor__4CA06362");

            entity.HasOne(d => d.Violator).WithMany(p => p.Violations)
                .HasForeignKey(d => d.ViolatorId)
                .HasConstraintName("FK__Violation__Viola__4D94879B");
        });

        modelBuilder.Entity<ViolationType>(entity =>
        {
            entity.HasKey(e => e.ViolationTypeId).HasName("PK__Violatio__3B1A4D7DB58694C6");

            entity.Property(e => e.ViolationTypeId).HasColumnName("ViolationTypeID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.StandardFine).HasColumnType("decimal(10, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
