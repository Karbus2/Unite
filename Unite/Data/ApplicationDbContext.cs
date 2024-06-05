using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Unite.Models;

namespace Unite.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<UserEvent> UserEvents { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        //void OnEntityStateChanged(object sender, EntityStateChangedEventArgs e)
        //{
        //    if (e.NewState == EntityState.Modified && e.Entry.Entity is IDbOperationTS entity)
        //        entity.UpdatedDate = DateTime.UtcNow;
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                      .HasDefaultValueSql("getdate()");
                entity.Property(e => e.UpdatedDate)
                      .HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.Scope)
                      .HasConversion<string>();
                entity.Property(e => e.CreatedDate)
                      .HasDefaultValueSql("getdate()");
                entity.Property(e => e.UpdatedDate)
                      .HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<UserEvent>(entity =>
            {
                entity.Property(e => e.Role)
                      .HasConversion<string>();
                entity.Property(e => e.State)
                      .HasConversion<string>();
                entity.HasKey(e => new {e.ParticipantId, e.EventId});
                entity.HasOne(e => e.Participant)
                      .WithMany(d => d.Events)
                      .HasForeignKey(e => e.ParticipantId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.ClientCascade);
                entity.HasOne(e => e.Event)
                      .WithMany(d => d.Participants)
                      .HasForeignKey(e => e.EventId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.ClientCascade);
                entity.Property(e => e.CreatedDate)
                      .HasDefaultValueSql("getdate()");
                entity.Property(e => e.UpdatedDate)
                      .HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<Friendship>(entity =>
            {
                entity.Property(e => e.State)
                      .HasConversion<string>();
                entity.HasKey(e => new { e.LeftSideId, e.RightSideId });
                entity.HasOne(e => e.LeftSide)
                      .WithMany(d => d.LeftSideFriendships)
                      .HasForeignKey(e => e.LeftSideId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.RightSide)
                      .WithMany(d => d.RightSideFriendships)
                      .HasForeignKey(e => e.RightSideId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.NoAction);
                entity.Property(e => e.CreatedDate)
                      .HasDefaultValueSql("getdate()");
                entity.Property(e => e.UpdatedDate)
                      .HasDefaultValueSql("getdate()");
            });
        }
    }
}
