namespace JokesApp.Server.Data.Errors
{
    /// <summary>
    /// Contiene i messaggi di errore utilizzati durante la fase di avvio (startup)
    /// dell'applicazione, principalmente legati alla connessione e al test del database.
    /// </summary>
    public static class StartupErrorMessages
    {
        #region Database

        /// <summary>
        /// Messaggio di errore generico quando l'applicazione non riesce a connettersi
        /// al database durante la fase di avvio.
        /// </summary>
        public const string DatabaseConnectionErrorOnStartup =
            "Impossibile connettersi al database all'avvio dell'applicazione.";

        /// <summary>
        /// Messaggio di errore generato quando il test del DbContext fallisce
        /// durante la fase di avvio dell'applicazione.
        /// </summary>
        public const string DbContextTestErrorOnStartup =
            "Errore durante il test del DbContext all'avvio dell'applicazione.";

        #endregion
    }
}
