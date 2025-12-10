// ============================================================================
// IMPORTAZIONI (using directives)
// ============================================================================
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// Modelli EF

// Value Objects
using JokesApp.Server.Domain.ValueObjects;

// Converters
using JokesApp.Server.Data.Converters;
using JokesApp.Server.Domain.Entities;

namespace JokesApp.Server.Data
{
    /// <summary>
    /// DbContext principale dell'applicazione. 
    /// Gestisce la persistenza delle entità Identity + Joke.
    /// </summary>
    public class JokesDbContext : IdentityDbContext<ApplicationUser>
    {
        public JokesDbContext(DbContextOptions<JokesDbContext> options)
            : base(options)
        {
        }

        // Tabelle
        public DbSet<Joke> Jokes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Necessario per Identity

            // -------------------------------------------------------------
            // CONFIGURAZIONE ENTITY: Joke
            // -------------------------------------------------------------
            var joke = modelBuilder.Entity<Joke>();

            // -------------------------------
            // JokeId (record struct <-> int)
            // -------------------------------
            joke.Property(j => j.Id)
                .HasConversion(new JokeIdConverter())
                .ValueGeneratedOnAdd()   // EF genera l'ID
                .HasColumnName("Id");

            // -------------------------------
            // ApplicationUserId (UserId <-> string)
            // -------------------------------
            joke.Property(j => j.ApplicationUserId)
                .HasConversion(new UserIdConverter())
                .HasMaxLength(450)
                .HasColumnName("ApplicationUserId")
                .IsRequired();

            // -------------------------------
            // Question (QuestionText <-> string)
            // -------------------------------
            joke.Property(j => j.Question)
                .HasConversion(new QuestionTextConverter())
                .HasMaxLength(200)
                .HasColumnName("Question")
                .IsRequired();

            // -------------------------------
            // Answer (AnswerText <-> string)
            // -------------------------------
            joke.Property(j => j.Answer)
                .HasConversion(new AnswerTextConverter())
                .HasMaxLength(500)
                .HasColumnName("Answer")
                .IsRequired();

            // -------------------------------
            // Relazione Joke ↔ ApplicationUser
            // -------------------------------
            joke.HasOne(j => j.Author)
                .WithMany(u => u.Jokes)
                .HasForeignKey(j => j.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------------
            // Likes (int)
            // -------------------------------
            joke.Property(j => j.Likes)
                .HasDefaultValue(0);

            // -------------------------------
            // DateTime
            // -------------------------------
            joke.Property(j => j.CreatedAt)
                .IsRequired();

            joke.Property(j => j.UpdatedAt)
                .IsRequired(false);
        }
    }
}
