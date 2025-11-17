using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class KanastaDbContext : DbContext
    {
        public KanastaDbContext(DbContextOptions<KanastaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Game> Games => Set<Game>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Round> Rounds => Set<Round>();
        public DbSet<RoundScore> RoundScores => Set<RoundScore>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>()
                .HasMany(g => g.Teams)
                .WithOne(t => t.Game)
                .HasForeignKey(t => t.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Game>()
                .HasMany(g => g.Rounds)
                .WithOne(r => r.Game)
                .HasForeignKey(r => r.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Round>()
                .HasMany(r => r.Scores)
                .WithOne(s => s.Round)
                .HasForeignKey(s => s.RoundId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.WinningTeam)
                .WithMany()
                .HasForeignKey(g => g.WinningTeamId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<RoundScore>()
                .HasIndex(s => new { s.RoundId, s.TeamId })
                .IsUnique();
        }
    }
}
