using System;
using Microsoft.EntityFrameworkCore;
using LOCC.Domain.Entities;

namespace LOCC.Infrastructure
{
    public class LoccDbContext : DbContext
    {
        public LoccDbContext(DbContextOptions<LoccDbContext> options) : base(options) { }

        public DbSet<Facility> Facilities => Set<Facility>();
        public DbSet<Resident> Residents => Set<Resident>();
        public DbSet<Staff> Staff => Set<Staff>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<OutbreakEvent> OutbreakEvents => Set<OutbreakEvent>();
        public DbSet<Case> Cases => Set<Case>();
        public DbSet<SymptomRecord> SymptomRecords => Set<SymptomRecord>();
        public DbSet<TestRecord> TestRecords => Set<TestRecord>();
        public DbSet<TaskAction> TaskActions => Set<TaskAction>();
        public DbSet<Resource> Resources => Set<Resource>();
        public DbSet<Communication> Communications => Set<Communication>();
        public DbSet<RecoveryBAU> RecoveryBAUs => Set<RecoveryBAU>();
        public DbSet<Alert> Alerts => Set<Alert>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<EvidenceSource> EvidenceSources => Set<EvidenceSource>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OutbreakEvent>()
                .HasKey(o => o.OutbreakId);

            modelBuilder.Entity<RecoveryBAU>()
                .HasKey(r => r.RecoveryId);

             modelBuilder.Entity<OutbreakEvent>()
                 .HasOne(o => o.RecoveryBAU)
                 .WithOne(r => r.OutbreakEvent)
                 .HasForeignKey<RecoveryBAU>(r => r.OutbreakId);
                
            modelBuilder.Entity<Facility>().HasKey(f => f.FacilityId);
            modelBuilder.Entity<Resident>().HasKey(r => r.ResidentId);
            modelBuilder.Entity<Staff>().HasKey(s => s.StaffId);
            modelBuilder.Entity<Location>().HasKey(l => l.LocationId);

            modelBuilder.Entity<OutbreakEvent>().HasKey(o => o.OutbreakId);
            modelBuilder.Entity<Case>().HasKey(c => c.CaseId);

            modelBuilder.Entity<SymptomRecord>().HasKey(s => s.SymptomId);
            modelBuilder.Entity<TestRecord>().HasKey(t => t.TestId);

            modelBuilder.Entity<TaskAction>().HasKey(t => t.TaskId);
            modelBuilder.Entity<Resource>().HasKey(r => r.ResourceId);

            modelBuilder.Entity<Communication>().HasKey(c => c.CommunicationId);
            modelBuilder.Entity<RecoveryBAU>().HasKey(r => r.RecoveryId);

            modelBuilder.Entity<EvidenceSource>().HasKey(e => e.EvidenceSourceId);
            modelBuilder.Entity<Alert>().HasKey(a => a.AlertId);
            modelBuilder.Entity<AuditLog>().HasKey(a => a.AuditLogId);

            base.OnModelCreating(modelBuilder);
            // Basic keys & relationships handled by convention.
        }
    }
}
