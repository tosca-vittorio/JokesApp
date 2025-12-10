namespace JokesApp.Server.Domain.Events
{
    /// <summary>
    /// Interfaccia marker per tutti gli eventi di dominio.
    /// </summary>
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
