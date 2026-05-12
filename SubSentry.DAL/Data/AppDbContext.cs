using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SubSentry.DAL.Entities;

namespace SubSentry.DAL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // These represent the tables that will be created in SQL Server
        public DbSet<User> Users { get; set; }
        public DbSet<SubscriptionTier> SubscriptionTiers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Here we can define FinTech-specific constraints, like ensuring money has the right precision.

            modelBuilder.Entity<SubscriptionTier>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            base.OnModelCreating(modelBuilder);
        }
    }
}