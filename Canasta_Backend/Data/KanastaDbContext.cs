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

            // -------------------------
            // GAME ↔ TEAMS (1:N)
            // -------------------------
            modelBuilder.Entity<Game>()
                .HasMany(g => g.Teams)
                .WithOne(t => t.Game)
                .HasForeignKey(t => t.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------
            // GAME ↔ ROUNDS (1:N)
            // -------------------------
            modelBuilder.Entity<Game>()
                .HasMany(g => g.Rounds)
                .WithOne(r => r.Game)
                .HasForeignKey(r => r.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------
            // ROUND ↔ SCORES (1:N)
            // Round törlése → Score-ok is törlődnek (OK)
            // -------------------------
            modelBuilder.Entity<Round>()
                .HasMany(r => r.Scores)
                .WithOne(s => s.Round)
                .HasForeignKey(s => s.RoundId)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------
            // TEAM ↔ SCORES (1:N)
            // FONTOS: NEM lehet Cascade → SQL hibát okoz
            // Restrict-re kell tenni!
            // -------------------------
            modelBuilder.Entity<RoundScore>()
                .HasOne(s => s.Team)
                .WithMany()
                .HasForeignKey(s => s.TeamId)
                .OnDelete(DeleteBehavior.Restrict);  // 🔥 FIX: NO CASCADE!

            // -------------------------
            // GAME ↔ WINNINGTEAM (1:1)
            // Restrict → így nem kavar be a cascade láncokba
            // -------------------------
            modelBuilder.Entity<Game>()
                .HasOne(g => g.WinningTeam)
                .WithMany()
                .HasForeignKey(g => g.WinningTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // ROUND-SCORE egyediség
            // -------------------------
            modelBuilder.Entity<RoundScore>()
                .HasIndex(s => new { s.RoundId, s.TeamId })
                .IsUnique();
        }
    }
}
