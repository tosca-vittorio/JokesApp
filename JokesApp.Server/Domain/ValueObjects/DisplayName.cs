using JokesApp.Server.Domain.Errors;
using JokesApp.Server.Domain.Exceptions;

namespace JokesApp.Server.Domain.ValueObjects
{
    /// <summary>
    /// Value Object che rappresenta il nome visuale (display name) di un utente.
    /// Immutabile, auto-validante e conforme alle regole del dominio.
    /// </summary>
    public sealed record DisplayName
    {
        /// <summary>
        /// Lunghezza massima consentita per il display name.
        /// </summary>
        public const int MaxLength = 50;

        /// <summary>
        /// Valore testuale interno del display name.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Indica se il valore rappresenta uno stato vuoto o non inizializzato.
        /// </summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

        /// <summary>
        /// Restituisce la lunghezza del testo interno.
        /// </summary>
        public int Length => Value.Length;

        /// <summary>
        /// Costruttore privato per garantire l'immutabilità
        /// e la validazione centralizzata tramite Create().
        /// </summary>
        private DisplayName(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method che valida e crea un nuovo Value Object
        /// conforme alle regole del dominio.
        /// </summary>
        /// <param name="value">Stringa contenente il display name.</param>
        /// <exception cref="DomainValidationException">
        /// Generata se il valore è nullo, vuoto o eccede la lunghezza massima consentita.
        /// </exception>
        public static DisplayName Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                // Nome visuale obbligatorio
                throw new DomainValidationException(ApplicationUserErrorMessages.DisplayNameRequired);
            }

            string v = value.Trim();

            if (v.Length > MaxLength)
            {
                // Lunghezza massima superata
                throw new DomainValidationException(ApplicationUserErrorMessages.DisplayNameMaxLength);
            }

            return new DisplayName(v);
        }

        /// <summary>
        /// Istanza vuota, utile per scenari di default, EF Core o binding iniziale.
        /// </summary>
        public static DisplayName Empty { get; } = new DisplayName(string.Empty);

        /// <summary>
        /// Restituisce il valore testuale del display name.
        /// </summary>
        public override string ToString() => Value;
    }
}
