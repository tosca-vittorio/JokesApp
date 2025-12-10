using JokesApp.Server.Domain.Errors;
using JokesApp.Server.Domain.Events;
using JokesApp.Server.Domain.Exceptions;
using JokesApp.Server.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JokesApp.Server.Domain.Entities
{
    /// <summary>
    /// Entità del dominio che rappresenta una barzelletta creata da un utente.
    /// Implementa logiche di dominio, validazione, gestione autore e generazione di eventi.
    /// Utilizza Value Objects per garantire integrità e coerenza dei dati.
    /// </summary>
    public class Joke
    {
        #region Domain events

        private readonly List<IDomainEvent> _domainEvents = new();

        /// <summary>
        /// Ritorna gli eventi di dominio attualmente registrati dall'entità.
        /// Non vengono serializzati né mappati dal livello di persistenza.
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        #endregion

        #region Properties

        /// <summary>
        /// Identificativo tipizzato della barzelletta.
        /// Viene generato automaticamente dal livello di persistenza al momento del salvataggio.
        /// </summary>
        public JokeId Id { get; private set; }

        /// <summary>
        /// Testo della domanda, rappresentato tramite un Value Object che garantisce lunghezza e validità.
        /// </summary>
        public QuestionText Question { get; private set; }

        /// <summary>
        /// Testo della risposta, rappresentato tramite un Value Object che garantisce lunghezza e validità.
        /// </summary>
        public AnswerText Answer { get; private set; }

        /// <summary>
        /// Identificatore dell'autore della barzelletta.
        /// È un Value Object che incapsula le regole di validazione del dominio.
        /// </summary>
        public UserId ApplicationUserId { get; private set; }

        /// <summary>
        /// Riferimento all'entità ApplicationUser lato dominio.
        /// Potrà essere popolata dal livello di persistenza o dall'application layer.
        /// </summary>
        public ApplicationUser? Author { get; private set; }

        /// <summary>
        /// Data e ora di creazione della barzelletta in formato UTC.
        /// </summary>
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Data e ora dell'ultima modifica della barzelletta in formato UTC.
        /// Null se non è mai stata aggiornata.
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Numero totale dei like ricevuti dalla barzelletta.
        /// </summary>
        public int Likes { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Costruttore protetto richiesto dagli strumenti di persistenza (es. EF Core).
        /// Non deve essere utilizzato manualmente nel codice di dominio.
        /// </summary>
        protected Joke()
        {
        }

        /// <summary>
        /// Costruttore principale del dominio.
        /// Esegue validazioni, assegna i Value Objects e genera un evento di creazione.
        /// </summary>
        /// <param name="question">Value Object contenente la domanda.</param>
        /// <param name="answer">Value Object contenente la risposta.</param>
        /// <param name="userId">Identificatore tipizzato dell'autore.</param>
        public Joke(QuestionText question, AnswerText answer, UserId userId)
        {
            EnsureQuestionAndAnswerAreDifferent(question, answer);

            Question = question;
            Answer = answer;
            ApplicationUserId = userId;
            CreatedAt = DateTime.UtcNow;

            // L’Id non esiste ancora (il data layer lo genererà), quindi inseriamo un placeholder.
            AddDomainEvent(new JokeWasCreated(
                JokeId.Empty,
                ApplicationUserId,
                Question,
                Answer,
                CreatedAt));
        }

        #endregion

        #region Domain event helpers

        /// <summary>
        /// Registra un nuovo evento di dominio associato all'entità.
        /// </summary>
        /// <param name="domainEvent">Evento di dominio da aggiungere.</param>
        /// <exception cref="ArgumentNullException">Generata se l'evento è nullo.</exception>
        private void AddDomainEvent(IDomainEvent domainEvent)
        {
            if (domainEvent is null)
            {
                throw new ArgumentNullException(nameof(domainEvent));
            }

            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Estrae tutti gli eventi di dominio e svuota la coda interna.
        /// Utilizzato dal dispatcher degli eventi nel livello applicativo.
        /// </summary>
        /// <returns>Collezione in sola lettura degli eventi estratti.</returns>
        public IReadOnlyCollection<IDomainEvent> PullDomainEvents()
        {
            var events = _domainEvents.ToList();
            _domainEvents.Clear();
            return events.AsReadOnly();
        }

        /// <summary>
        /// Elimina tutti gli eventi accumulati senza restituirli.
        /// </summary>
        public void ClearDomainEvents() => _domainEvents.Clear();

        #endregion

        #region Author management

        /// <summary>
        /// Imposta l'autore della barzelletta verificando che:
        /// - l'istanza non sia nulla;
        /// - non sia già stato impostato un autore;
        /// - l'identificativo dell'autore corrisponda a quello previsto dal dominio.
        /// </summary>
        /// <param name="author">Istanza di <see cref="ApplicationUser"/> da associare.</param>
        public void SetAuthor(ApplicationUser author)
        {
            if (author is null)
            {
                throw new DomainValidationException(
                    JokeErrorMessages.AuthorNull,
                    nameof(author));
            }

            if (Author is not null)
            {
                throw new DomainOperationException(JokeErrorMessages.AuthorAlreadySet);
            }

            if (author.Id != ApplicationUserId.Value)
            {
                throw new DomainValidationException(
                    JokeErrorMessages.AuthorIdMismatch,
                    nameof(author));
            }

            Author = author;
        }

        #endregion

        #region Domain behavior

        /// <summary>
        /// Determina se la barzelletta è stata creata dal determinato utente.
        /// </summary>
        /// <param name="userId">Identificatore dell'utente da verificare.</param>
        public bool IsAuthoredBy(UserId userId)
            => ApplicationUserId.Equals(userId);

        /// <summary>
        /// Aggiorna la barzelletta sostituendo domanda e risposta dopo le opportune validazioni.
        /// Genera un evento di aggiornamento.
        /// </summary>
        /// <param name="userId">Identificatore dell'utente che richiede l'aggiornamento.</param>
        /// <param name="question">Nuovo testo della domanda.</param>
        /// <param name="answer">Nuovo testo della risposta.</param>
        public void Update(UserId userId, QuestionText question, AnswerText answer)
        {
            if (!IsAuthoredBy(userId))
            {
                throw new UnauthorizedDomainOperationException(JokeErrorMessages.UpdateNotAllowed);
            }

            EnsureQuestionAndAnswerAreDifferent(question, answer);

            Question = question;
            Answer = answer;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new JokeWasUpdated(
                Id,
                Question,
                Answer,
                UpdatedAt.Value));
        }

        /// <summary>
        /// Incrementa il numero di like garantendo che non si verifichino overflow.
        /// Genera un evento di like.
        /// </summary>
        public void AddLike()
        {
            if (Likes == int.MaxValue)
            {
                throw new DomainOperationException(JokeErrorMessages.MaximumLikeOfJokeReached);
            }

            Likes++;

            AddDomainEvent(new JokeWasLiked(
                Id,
                Likes));
        }

        /// <summary>
        /// Decrementa il numero di like garantendo che non scenda sotto zero.
        /// Genera un evento di dislike.
        /// </summary>
        public void RemoveLike()
        {
            if (Likes == 0)
            {
                throw new DomainOperationException(JokeErrorMessages.MinimumLikeOfJokeReached);
            }

            Likes--;

            AddDomainEvent(new JokeWasUnliked(
                Id,
                Likes));
        }

        #endregion

        #region Validation helpers

        /// <summary>
        /// Verifica che domanda e risposta non siano identiche ignorando le differenze di maiuscole/minuscole.
        /// </summary>
        /// <param name="q">Testo della domanda.</param>
        /// <param name="a">Testo della risposta.</param>
        private static void EnsureQuestionAndAnswerAreDifferent(QuestionText q, AnswerText a)
        {
            if (string.Equals(q.Value, a.Value, StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainValidationException(
                    JokeErrorMessages.QuestionAndAnswerCannotMatch);
            }
        }

        /// <summary>
        /// Controlla che lo stato interno dell'entità sia coerente e valido.
        /// Utile per test, importazioni o verifiche interne.
        /// </summary>
        public void ValidateIntegrity()
        {
            EnsureQuestionAndAnswerAreDifferent(Question, Answer);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Ritorna una rappresentazione testuale sintetica dell'entità utile per logging o debugging.
        /// </summary>
        public override string ToString()
            => $"Joke(Id={Id}, UserId={ApplicationUserId}, CreatedAt={CreatedAt:O})";

        #endregion
    }
}
