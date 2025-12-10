namespace JokesApp.Server.Domain.Events
{
    /// <summary>
    /// Implementazione base di un evento di dominio.
    /// Contiene la data/ora di occorrenza.
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; }

        protected DomainEvent()
        {
            OccurredOn = DateTime.UtcNow;
        }
    }
}
