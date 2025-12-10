using JokesApp.Server.Domain.ValueObjects;
using JokesApp.Server.Domain.Errors;
using JokesApp.Server.Domain.Exceptions;

namespace JokesApp.Server.Domain.Events
{
    /// <summary>
    /// Evento di dominio che rappresenta l’avvenuto aggiornamento di una barzelletta.
    /// Registra i nuovi valori e il timestamp dell'operazione.
    /// </summary>
    public sealed class JokeWasUpdated : IDomainEvent
    {
        /// <summary>
        /// Identificativo tipizzato della barzelletta aggiornata.
        /// </summary>
        public JokeId JokeId { get; }

        /// <summary>
        /// Nuova domanda della barzelletta.
        /// </summary>
        public QuestionText NewQuestion { get; }

        /// <summary>
        /// Nuova risposta della barzelletta.
        /// </summary>
        public AnswerText NewAnswer { get; }

        /// <summary>
        /// Timestamp UTC dell'aggiornamento dell'entità.
        /// </summary>
        public DateTime UpdatedAt { get; }

        /// <summary>
        /// Timestamp UTC di generazione dell'evento.
        /// </summary>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        /// <summary>
        /// Costruisce un evento di aggiornamento coerente secondo le regole del dominio.
        /// </summary>
        public JokeWasUpdated(
            JokeId jokeId,
            QuestionText newQuestion,
            AnswerText newAnswer,
            DateTime updatedAt)
        {
            if (jokeId.IsEmpty)
                throw new DomainValidationException(JokeErrorMessages.JokeIdEmpty);

            NewQuestion = newQuestion
                ?? throw new DomainValidationException(JokeErrorMessages.QuestionNullOrEmpty);

            NewAnswer = newAnswer
                ?? throw new DomainValidationException(JokeErrorMessages.AnswerNullOrEmpty);

            if (updatedAt == default)
                throw new DomainValidationException("UpdatedAt timestamp is invalid.");

            if (updatedAt.Kind != DateTimeKind.Utc)
                updatedAt = updatedAt.ToUniversalTime();

            JokeId = jokeId;
            UpdatedAt = updatedAt;
        }

        /// <summary>
        /// Rappresentazione leggibile per log e debugging.
        /// </summary>
        public override string ToString()
            => $"[JokeWasUpdated] JokeId={JokeId}, At={UpdatedAt:O}, OccurredOn={OccurredOn:O}";
    }
}
