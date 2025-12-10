using JokesApp.Server.Domain.ValueObjects;
using JokesApp.Server.Domain.Errors;
using JokesApp.Server.Domain.Exceptions;

namespace JokesApp.Server.Domain.Events
{
    /// <summary>
    /// Evento di dominio che indica la creazione di una nuova barzelletta.
    /// Registra i principali dati di stato iniziale dell'entità.
    /// </summary>
    public sealed class JokeWasCreated : IDomainEvent
    {
        /// <summary>
        /// Identificativo tipizzato della barzelletta appena creata.
        /// </summary>
        public JokeId JokeId { get; }

        /// <summary>
        /// Identificativo dell'autore che ha creato la barzelletta.
        /// </summary>
        public UserId AuthorId { get; }

        /// <summary>
        /// Testo della domanda fornita al momento della creazione.
        /// </summary>
        public QuestionText Question { get; }

        /// <summary>
        /// Testo della risposta fornita al momento della creazione.
        /// </summary>
        public AnswerText Answer { get; }

        /// <summary>
        /// Timestamp UTC in cui la barzelletta è stata creata.
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// Timestamp UTC in cui l'evento è stato generato.
        /// </summary>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        /// <summary>
        /// Crea un evento di creazione completamente valido assicurando
        /// coerenza e correttezza dei dati di dominio.
        /// </summary>
        public JokeWasCreated(
            JokeId jokeId,
            UserId authorId,
            QuestionText question,
            AnswerText answer,
            DateTime createdAt)
        {
            if (jokeId.IsEmpty)
                throw new DomainValidationException(JokeErrorMessages.JokeIdEmpty);

            if (authorId.IsEmpty)
                throw new DomainValidationException(JokeErrorMessages.UserIdNullOrEmpty);

            Question = question
                ?? throw new DomainValidationException(JokeErrorMessages.QuestionNullOrEmpty);

            Answer = answer
                ?? throw new DomainValidationException(JokeErrorMessages.AnswerNullOrEmpty);

            if (createdAt == default)
                throw new DomainValidationException("CreatedAt timestamp is invalid.");

            if (createdAt.Kind != DateTimeKind.Utc)
                createdAt = createdAt.ToUniversalTime();

            JokeId = jokeId;
            AuthorId = authorId;
            CreatedAt = createdAt;
        }

        /// <summary>
        /// Fornisce una rappresentazione stringa leggibile dell'evento.
        /// </summary>
        public override string ToString()
            => $"[JokeWasCreated] JokeId={JokeId}, AuthorId={AuthorId}, At={CreatedAt:O}, OccurredOn={OccurredOn:O}";
    }
}
