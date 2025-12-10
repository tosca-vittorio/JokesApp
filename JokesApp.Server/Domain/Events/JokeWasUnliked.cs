using JokesApp.Server.Domain.ValueObjects;
using JokesApp.Server.Domain.Errors;
using JokesApp.Server.Domain.Exceptions;

namespace JokesApp.Server.Domain.Events
{
    /// <summary>
    /// Evento di dominio che indica che una barzelletta ha ricevuto un "unlike".
    /// Contiene il nuovo conteggio dei like e il timestamp dell'operazione.
    /// </summary>
    public sealed class JokeWasUnliked : IDomainEvent
    {
        /// <summary>
        /// Identificativo tipizzato della barzelletta coinvolta nell'evento.
        /// </summary>
        public JokeId JokeId { get; }

        /// <summary>
        /// Conteggio totale dei like dopo la rimozione.
        /// </summary>
        public int LikesAfterChange { get; }

        /// <summary>
        /// Timestamp UTC in cui l'evento è stato generato.
        /// </summary>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        /// <summary>
        /// Crea un evento di "unlike" conforme alle regole del dominio.
        /// </summary>
        public JokeWasUnliked(JokeId jokeId, int likesAfterChange)
        {
            if (jokeId.IsEmpty)
                throw new DomainValidationException(JokeErrorMessages.JokeIdEmpty);

            if (likesAfterChange < 0)
                throw new DomainValidationException(JokeErrorMessages.MinimumLikeOfJokeReached);

            JokeId = jokeId;
            LikesAfterChange = likesAfterChange;
        }

        public override string ToString()
            => $"[JokeWasUnliked] JokeId={JokeId}, Likes={LikesAfterChange}, At={OccurredOn:O}";
    }
}
