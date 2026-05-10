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
            base.OnModelCreating(modelBuilder);
            // Basic keys & relationships handled by convention.
        }
    }
}
