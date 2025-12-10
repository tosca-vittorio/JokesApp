using System;

namespace JokesApp.Server.Domain.Exceptions
{
    /// <summary>
    /// Eccezione che rappresenta un'operazione di dominio non valida
    /// rispetto allo stato corrente dell'aggregato o alle regole di business.
    /// </summary>
    public class DomainOperationException : DomainException
    {
        #region Constructors

        /// <summary>
        /// Crea una nuova istanza di <see cref="DomainOperationException"/>.
        /// </summary>
        public DomainOperationException()
        {
        }

        /// <summary>
        /// Crea una nuova istanza di <see cref="DomainOperationException"/> con un messaggio descrittivo.
        /// </summary>
        /// <param name="message">Messaggio descrittivo dell'errore di operazione di dominio.</param>
        public DomainOperationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Crea una nuova istanza di <see cref="DomainOperationException"/> con un messaggio descrittivo
        /// e una eccezione interna che ha causato l'errore corrente.
        /// </summary>
        /// <param name="message">Messaggio descrittivo dell'errore di operazione di dominio.</param>
        /// <param name="innerException">Eccezione che ha causato l'errore corrente.</param>
        public DomainOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
            // Preserve original exception details and stack trace.
        }

        #endregion
    }
}
