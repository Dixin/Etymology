using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Etymology.Data.Models
{
    public partial class EtymologyContext : DbContext
    {
        public EtymologyContext(DbContextOptions<EtymologyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bronze> Bronze { get; set; }
        public virtual DbSet<Etymology> Etymology { get; set; }
        public virtual DbSet<Liushutong> Liushutong { get; set; }
        public virtual DbSet<Oracle> Oracle { get; set; }
        public virtual DbSet<Seal> Seal { get; set; }
        public virtual DbSet<VEtymology> VEtymology { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bronze>(entity =>
            {
                entity.HasKey(e => e.BronzeId)
                    .IsClustered(false);

                entity.HasIndex(e => e.Traditional)
                    .IsClustered();
            });

            modelBuilder.Entity<Etymology>(entity =>
            {
                entity.HasKey(e => e.EtymologyId)
                    .IsClustered(false);

                entity.HasIndex(e => e.OldTraditional);

                entity.HasIndex(e => e.Simplified);

                entity.HasIndex(e => e.Traditional)
                    .IsUnique()
                    .IsClustered();
            });

            modelBuilder.Entity<Liushutong>(entity =>
            {
                entity.HasKey(e => e.LiushutongId)
                    .IsClustered(false);

                entity.HasIndex(e => e.Traditional)
                    .IsClustered();

                entity.HasOne(d => d.Seal)
                    .WithMany(p => p.Liushutong)
                    .HasForeignKey(d => d.SealId)
                    .HasConstraintName("FK_Liushutong_Seal");
            });

            modelBuilder.Entity<Oracle>(entity =>
            {
                entity.HasKey(e => e.OracleId)
                    .IsClustered(false);

                entity.HasIndex(e => e.Traditional)
                    .IsClustered();
            });

            modelBuilder.Entity<Seal>(entity =>
            {
                entity.HasKey(e => e.SealId)
                    .IsClustered(false);

                entity.HasIndex(e => e.Traditional)
                    .IsClustered();
            });

            modelBuilder.Entity<VEtymology>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vEtymology");

                entity.Property(e => e.EtymologyId).ValueGeneratedOnAdd();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
