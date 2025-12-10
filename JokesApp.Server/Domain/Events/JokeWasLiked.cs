using JokesApp.Server.Domain.ValueObjects;
using JokesApp.Server.Domain.Errors;
using JokesApp.Server.Domain.Exceptions;

namespace JokesApp.Server.Domain.Events
{
    /// <summary>
    /// Evento di dominio che rappresenta l'aggiunta di un like a una barzelletta.
    /// Registra l'ID della barzelletta, il nuovo totale dei like e il timestamp dell'evento.
    /// </summary>
    public sealed class JokeWasLiked : IDomainEvent
    {
        /// <summary>
        /// Identificativo tipizzato della barzelletta a cui è stato aggiunto un like.
        /// </summary>
        public JokeId JokeId { get; }

        /// <summary>
        /// Numero totale dei like dopo l'operazione.
        /// </summary>
        public int LikesAfterChange { get; }

        /// <summary>
        /// Timestamp UTC in cui l'evento è stato generato.
        /// </summary>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        /// <summary>
        /// Crea un evento di like valido secondo le regole di dominio.
        /// </summary>
        public JokeWasLiked(JokeId jokeId, int likesAfterChange)
        {
            if (jokeId.IsEmpty)
                throw new DomainValidationException(JokeErrorMessages.JokeIdEmpty);

            if (likesAfterChange < 0)
                throw new DomainValidationException(JokeErrorMessages.MinimumLikeOfJokeReached);

            JokeId = jokeId;
            LikesAfterChange = likesAfterChange;
        }

        public override string ToString()
            => $"[JokeWasLiked] JokeId={JokeId}, Likes={LikesAfterChange}, At={OccurredOn:O}";
    }
}
