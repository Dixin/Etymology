namespace Etymology.Data.Models
{
    using Microsoft.EntityFrameworkCore;

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
                entity.HasIndex(e => e.Character);

                entity.Property(e => e.BronzeId).ValueGeneratedNever();
            });

            modelBuilder.Entity<Etymology>(entity =>
            {
                entity.Property(e => e.Traditional).ValueGeneratedNever();
            });

            modelBuilder.Entity<Liushutong>(entity =>
            {
                entity.Property(e => e.LiushutongId).ValueGeneratedNever();

                entity.HasOne(d => d.Seal)
                    .WithMany(p => p.Liushutong)
                    .HasForeignKey(d => d.SealId)
                    .HasConstraintName("FK_Liushutong_Seal");
            });

            modelBuilder.Entity<Oracle>(entity =>
            {
                entity.HasIndex(e => e.Character);

                entity.Property(e => e.OracleId).ValueGeneratedNever();
            });

            modelBuilder.Entity<Seal>(entity =>
            {
                entity.Property(e => e.SealId).ValueGeneratedNever();
            });
        }
    }
}
