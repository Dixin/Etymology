using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Etymology.Data.Models
{
    public partial class EtymologyContext : DbContext
    {
        public virtual DbSet<Bronze> Bronze { get; set; }
        public virtual DbSet<Etymology> Etymology { get; set; }
        public virtual DbSet<Liushutong> Liushutong { get; set; }
        public virtual DbSet<Oracle> Oracle { get; set; }
        public virtual DbSet<Seal> Seal { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bronze>(entity =>
            {
                entity.HasIndex(e => e.Traditional)
                    .ForSqlServerIsClustered();
            });

            modelBuilder.Entity<Etymology>(entity =>
            {
                entity.HasIndex(e => e.Traditional)
                    .IsUnique()
                    .ForSqlServerIsClustered();
            });

            modelBuilder.Entity<Liushutong>(entity =>
            {
                entity.HasIndex(e => e.Traditional)
                    .ForSqlServerIsClustered();

                entity.HasOne(d => d.Seal)
                    .WithMany(p => p.Liushutong)
                    .HasForeignKey(d => d.SealId)
                    .HasConstraintName("FK_Liushutong_Seal");
            });

            modelBuilder.Entity<Oracle>(entity =>
            {
                entity.HasIndex(e => e.Traditional)
                    .ForSqlServerIsClustered();
            });

            modelBuilder.Entity<Seal>(entity =>
            {
                entity.HasIndex(e => e.Traditional)
                    .ForSqlServerIsClustered();
            });
        }
    }
}
