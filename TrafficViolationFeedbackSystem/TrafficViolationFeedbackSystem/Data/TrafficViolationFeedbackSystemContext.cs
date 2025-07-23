using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem.Data;

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

    public virtual DbSet<Fine> Fines { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<Violation> Violations { get; set; }

    public virtual DbSet<ViolationType> ViolationTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=TrafficViolationFeedbackSystem;User ID=sa;Password=123;Encrypt=False;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appeal>(entity =>
        {
            entity.HasKey(e => e.AppealId).HasName("PK__Appeals__BB684E103333E502");

            entity.Property(e => e.AppealId).HasColumnName("AppealID");
            entity.Property(e => e.Result).HasMaxLength(50);
            entity.Property(e => e.SubmitDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ViolationId).HasColumnName("ViolationID");
            entity.Property(e => e.ViolatorId).HasColumnName("ViolatorID");

            entity.HasOne(d => d.Violation).WithMany(p => p.Appeals)
                .HasForeignKey(d => d.ViolationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appeals_Violation");

            entity.HasOne(d => d.Violator).WithMany(p => p.Appeals)
                .HasForeignKey(d => d.ViolatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appeals_Violator");
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("PK__Attachme__442C64DE31D15AA1");

            entity.Property(e => e.AttachmentId).HasColumnName("AttachmentID");
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.FileType).HasMaxLength(10);
            entity.Property(e => e.ReportId).HasColumnName("ReportID");

            entity.HasOne(d => d.Report).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attachments_Report");
        });

        modelBuilder.Entity<Fine>(entity =>
        {
            entity.HasKey(e => e.FineId).HasName("PK__Fines__9D4A9BCC60E234A2");

            entity.Property(e => e.FineId).HasColumnName("FineID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TransactionCode).HasMaxLength(100);
            entity.Property(e => e.ViolationId).HasColumnName("ViolationID");

            entity.HasOne(d => d.ConfirmedByNavigation).WithMany(p => p.FineConfirmedByNavigations)
                .HasForeignKey(d => d.ConfirmedBy)
                .HasConstraintName("FK_Fines_ConfirmedBy");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.FineCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Fines_CreatedBy");

            entity.HasOne(d => d.Violation).WithMany(p => p.Fines)
                .HasForeignKey(d => d.ViolationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fines_Violation");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Reports__D5BD48E556AC5491");

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
                .HasConstraintName("FK_Reports_ProcessedBy");

            entity.HasOne(d => d.Reporter).WithMany(p => p.ReportReporters)
                .HasForeignKey(d => d.ReporterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reports_Reporter");

            entity.HasOne(d => d.ViolationType).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ViolationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reports_ViolationType");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC64EB6BFD");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534FC9353C7").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.ResetCode)
                .HasMaxLength(7)
                .IsUnicode(false);
            entity.Property(e => e.ResetCodeExpiry).HasColumnType("datetime");
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicles__476B54B2D52020B8");

            entity.HasIndex(e => e.PlateNumber, "UQ__Vehicles__03692624D6D83F72").IsUnique();

            entity.Property(e => e.VehicleId).HasColumnName("VehicleID");
            entity.Property(e => e.Brand).HasMaxLength(50);
            entity.Property(e => e.Model).HasMaxLength(50);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.PlateNumber).HasMaxLength(15);

            entity.HasOne(d => d.Owner).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicles_Owner");
        });

        modelBuilder.Entity<Violation>(entity =>
        {
            entity.HasKey(e => e.ViolationId).HasName("PK__Violatio__18B6DC2874229909");

            entity.Property(e => e.ViolationId).HasColumnName("ViolationID");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.FineAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FineDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReportId).HasColumnName("ReportID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.ViolatorId).HasColumnName("ViolatorID");

            entity.HasOne(d => d.Report).WithMany(p => p.Violations)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Violations_Report");

            entity.HasOne(d => d.Violator).WithMany(p => p.Violations)
                .HasForeignKey(d => d.ViolatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Violations_Violator");
        });

        modelBuilder.Entity<ViolationType>(entity =>
        {
            entity.HasKey(e => e.ViolationTypeId).HasName("PK__Violatio__3B1A4D7D1158FF93");

            entity.Property(e => e.ViolationTypeId).HasColumnName("ViolationTypeID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.StandardFine).HasColumnType("decimal(10, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
