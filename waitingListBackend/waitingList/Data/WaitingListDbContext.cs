using Microsoft.EntityFrameworkCore;
using waitingList.Models;

namespace waitingList.Data
{
    public class WaitingListDbContext(DbContextOptions<WaitingListDbContext> options)
        : DbContext(options)
    {
        public DbSet<Client> clients => Set<Client>();
        public DbSet<UnitCentre> unitCentres => Set<UnitCentre>();
        public DbSet<Visit> visits => Set<Visit>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(client => client.clientId);

                entity.HasIndex(client => client.accNumber)
                    .IsUnique();

                entity.Property(client => client.accNumber)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(client => client.fullName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(client => client.cellNumber)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(client => client.status)
                    .HasConversion<string>()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<UnitCentre>(entity =>
            {
                entity.HasKey(unitCentre => unitCentre.centreId);

                entity.HasIndex(unitCentre => unitCentre.externalCentreId)
                    .IsUnique();

                entity.Property(unitCentre => unitCentre.externalCentreId)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(unitCentre => unitCentre.centreName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(unitCentre => unitCentre.address)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(unitCentre => unitCentre.latitude)
                    .HasPrecision(9, 6);

                entity.Property(unitCentre => unitCentre.longitude)
                    .HasPrecision(9, 6);

                entity.Property(unitCentre => unitCentre.allowedRadiusMetres)
                    .HasDefaultValue(150)
                    .IsRequired();
            });

            modelBuilder.Entity<Visit>(entity =>
            {
                entity.HasKey(visit => visit.visitId);

                entity.HasIndex(visit => new
                {
                    visit.clientId,
                    visit.centreId,
                    visit.visitDate
                }).IsUnique();

                entity.Property(visit => visit.status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.HasIndex(visit => visit.qrToken)
                    .IsUnique();

                entity.Property(visit => visit.qrToken)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasOne(visit => visit.client)
                    .WithMany(client => client.visits)
                    .HasForeignKey(visit => visit.clientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(visit => visit.centre)
                    .WithMany(centre => centre.visits)
                    .HasForeignKey(visit => visit.centreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
