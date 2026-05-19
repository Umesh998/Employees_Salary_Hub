using Employees_Salary_Hub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Employees_Salary_Hub.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<SalarySlip> SalarySlips { get; set; }
        public DbSet<PayrollBatch> PayrollBatches { get; set; }
        public DbSet<OtpRecord> OtpRecords { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ── SalarySlip ──────────────────────────────────────────
            builder.Entity<SalarySlip>(entity =>
            {
                // Unique constraint: one slip per employee per month/year
                entity.HasIndex(s => new { s.UserId, s.PayrollBatchId })
                      .IsUnique()
                      .HasDatabaseName("IX_SalarySlip_User_Batch");

                entity.HasOne(s => s.User)
                      .WithMany(u => u.SalarySlips)
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.PayrollBatch)
                      .WithMany(b => b.SalarySlips)
                      .HasForeignKey(s => s.PayrollBatchId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(s => s.BasicSalary).HasPrecision(18, 2);
                entity.Property(s => s.HRA).HasPrecision(18, 2);
                entity.Property(s => s.Bonus).HasPrecision(18, 2);
                entity.Property(s => s.PF).HasPrecision(18, 2);
                entity.Property(s => s.Tax).HasPrecision(18, 2);
                entity.Property(s => s.NetSalary).HasPrecision(18, 2);
            });

            // ── PayrollBatch ────────────────────────────────────────
            builder.Entity<PayrollBatch>(entity =>
            {
                entity.HasIndex(b => new { b.Month, b.Year })
                      .IsUnique()
                      .HasDatabaseName("IX_PayrollBatch_Month_Year");
            });

            // ── OtpRecord ───────────────────────────────────────────
            builder.Entity<OtpRecord>(entity =>
            {
                entity.HasOne(o => o.User)
                      .WithMany(u => u.OtpRecords)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── AuditLog ────────────────────────────────────────────
            builder.Entity<AuditLog>(entity =>
            {
                entity.HasOne(a => a.User)
                      .WithMany(u => u.AuditLogs)
                      .HasForeignKey(a => a.UserId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ── ApplicationUser ─────────────────────────────────────
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(u => u.EmployeeCode)
                      .IsUnique()
                      .HasDatabaseName("IX_User_EmployeeCode");
            });
        }
    }
}
