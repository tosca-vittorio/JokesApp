using JokesApp.Server.Domain.Errors;
using JokesApp.Server.Domain.Exceptions;

namespace JokesApp.Server.Domain.ValueObjects
{
    /// <summary>
    /// Identificatore tipizzato e immutabile per la barzelletta.
    /// Deve rappresentare sempre un intero positivo e valido nel dominio.
    /// </summary>
    public readonly record struct JokeId
    {
        #region Properties

        /// <summary>
        /// Valore numerico dell'identificatore.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Indica se l'identificatore rappresenta uno stato non inizializzato
        /// o non valido (0 o qualsiasi valore non positivo).
        /// </summary>
        public bool IsEmpty => Value <= 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Costruttore privato: nel codice applicativo la creazione dovrebbe passare
        /// tramite <see cref="Create(int)"/> oppure tramite <see cref="Empty"/>.
        /// Come per tutti gli struct in C#, esiste comunque un costruttore di default
        /// che produce uno stato equivalente a <see cref="Empty"/> (Value &lt;= 0).
        /// </summary>

        private JokeId(int value)
        {
            Value = value;
        }

        #endregion

        #region Factory

        /// <summary>
        /// Crea un identificatore di barzelletta valido, garantendo che sia strettamente positivo.
        /// </summary>
        /// <param name="value">Valore numerico da utilizzare come identificatore.</param>
        /// <returns>Un'istanza valida di <see cref="JokeId"/>.</returns>
        /// <exception cref="DomainValidationException">
        /// Generata quando il valore è minore o uguale a zero.
        /// </exception>
        public static JokeId Create(int value)
        {
            if (value <= 0)
            {
                throw new DomainValidationException(
                    JokeErrorMessages.JokeIdInvalid,
                    nameof(JokeId));
            }

            // At this point, the identifier is a valid domain value.
            return new JokeId(value);
        }

        #endregion

        #region Static members

        /// <summary>
        /// Identificatore "vuoto", utilizzato come placeholder iniziale
        /// (ad esempio prima che Entity Framework assegni il valore reale).
        /// </summary>
        public static JokeId Empty { get; } = new JokeId(0);

        #endregion

        #region Overrides

        /// <summary>
        /// Restituisce una rappresentazione testuale dell'identificatore.
        /// </summary>
        public override string ToString() => Value.ToString();

        #endregion
    }
}