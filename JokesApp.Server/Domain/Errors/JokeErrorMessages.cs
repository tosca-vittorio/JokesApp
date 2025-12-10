namespace JokesApp.Server.Domain.Errors
{
    /// <summary>
    /// Contiene tutti i messaggi di errore relativi al dominio Joke.
    /// Questi messaggi vengono utilizzati da entità, Value Object ed eventi
    /// per mantenere consistenza e centralità delle regole di validazione.
    /// </summary>
    public static class JokeErrorMessages
    {
        #region Question errors

        /// <summary>
        /// Messaggio per indicare che la domanda è nulla o vuota.
        /// </summary>
        public const string QuestionNullOrEmpty =
            "Question cannot be null or empty.";

        /// <summary>
        /// Messaggio per indicare che la domanda supera la lunghezza massima consentita.
        /// </summary>
        public const string QuestionTooLong =
            "Question exceeds maximum allowed length.";

        #endregion

        #region Answer errors

        /// <summary>
        /// Messaggio per indicare che la risposta è nulla o vuota.
        /// </summary>
        public const string AnswerNullOrEmpty =
            "Answer cannot be null or empty.";

        /// <summary>
        /// Messaggio per indicare che la risposta supera la lunghezza massima consentita.
        /// </summary>
        public const string AnswerTooLong =
            "Answer exceeds maximum allowed length.";

        #endregion

        #region User / Author errors

        /// <summary>
        /// Messaggio per indicare che l'identificativo utente è nullo o vuoto.
        /// </summary>
        public const string UserIdNullOrEmpty =
            "UserId cannot be null or empty.";

        /// <summary>
        /// Messaggio per indicare che l'identificativo utente contiene caratteri non validi.
        /// </summary>
        public const string UserIdInvalid =
            "UserId contains invalid characters.";

        /// <summary>
        /// Messaggio per indicare che l'identificativo utente supera la lunghezza massima consentita.
        /// </summary>
        public const string UserIdTooLong =
            "UserId exceeds maximum allowed length.";

        /// <summary>
        /// Messaggio per indicare che l'autore non può essere nullo.
        /// </summary>
        public const string AuthorNull =
            "The author cannot be null.";

        /// <summary>
        /// Messaggio per indicare che l'autore è già stato assegnato.
        /// </summary>
        public const string AuthorAlreadySet =
            "The author has already been assigned.";

        /// <summary>
        /// Messaggio per indicare che l'AuthorId non corrisponde allo UserId della joke.
        /// </summary>
        public const string AuthorIdMismatch =
            "AuthorId does not match the Joke's UserId.";

        #endregion

        #region JokeId errors (Value Object)

        /// <summary>
        /// Messaggio per indicare che il JokeId non è un intero positivo.
        /// </summary>
        public const string JokeIdInvalid =
            "JokeId must be a positive integer.";

        /// <summary>
        /// Messaggio per indicare che il JokeId è vuoto o non valorizzato.
        /// </summary>
        public const string JokeIdEmpty =
            "JokeId cannot be empty.";

        #endregion

        #region Domain rule violations

        /// <summary>
        /// Messaggio per indicare che domanda e risposta non possono essere identiche.
        /// </summary>
        public const string QuestionAndAnswerCannotMatch =
            "Question and Answer cannot be identical.";

        /// <summary>
        /// Messaggio per indicare che è stato raggiunto il numero massimo di like.
        /// </summary>
        public const string MaximumLikeOfJokeReached =
            "Maximum number of likes has been reached.";

        /// <summary>
        /// Messaggio per indicare che il numero di like non può scendere sotto zero.
        /// </summary>
        public const string MinimumLikeOfJokeReached =
            "Like count cannot go below zero.";

        /// <summary>
        /// Messaggio per indicare che l'utente non è autorizzato ad aggiornare la joke.
        /// </summary>
        public const string UpdateNotAllowed =
            "The user is not authorized to update this joke.";

        #endregion

        #region Generic

        /// <summary>
        /// Messaggio generico per indicare che un valore obbligatorio è mancante.
        /// </summary>
        public const string ValueRequired =
            "A required value was missing.";

        #endregion
    }
}
