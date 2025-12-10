using System;

namespace JokesApp.Server.Domain.Exceptions
{
    /// <summary>
    /// Eccezione generata quando un utente tenta una operazione di dominio
    /// senza possedere i permessi necessari.
    /// </summary>
    public class UnauthorizedDomainOperationException : DomainOperationException
    {
        #region Constructors

        /// <summary>
        /// Crea una nuova istanza di <see cref="UnauthorizedDomainOperationException"/>.
        /// </summary>
        public UnauthorizedDomainOperationException()
        {
        }

        /// <summary>
        /// Crea una nuova istanza di <see cref="UnauthorizedDomainOperationException"/> con un messaggio descrittivo.
        /// </summary>
        /// <param name="message">Messaggio descrittivo dell'errore di autorizzazione.</param>
        public UnauthorizedDomainOperationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Crea una nuova istanza di <see cref="UnauthorizedDomainOperationException"/> con un messaggio descrittivo
        /// e una eccezione interna che ha causato l'errore corrente.
        /// </summary>
        /// <param name="message">Messaggio descrittivo dell'errore di autorizzazione.</param>
        /// <param name="innerException">Eccezione che ha causato l'errore corrente.</param>
        public UnauthorizedDomainOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
            // Preserve original exception details and stack trace.
        }

        #endregion
    }
}
