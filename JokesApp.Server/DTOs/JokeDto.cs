using System.ComponentModel.DataAnnotations;

namespace JokesApp.Server.DTOs
{
    // DTO per la creazione e l'aggiornamento delle barzellette
    // Separa il modello di dominio (Joke) da ciò che viene esposto/ricevuto via API
    public class JokeDto
    {
        public int Id { get; set; }


        // Se il client non invia questo campo, il ModelState sarà invalido
        // e il controller restituirà 400 Bad Request PRIMA di toccare il DB.
        [Required(ErrorMessage = "La domanda è obbligatoria.")]
        [MaxLength(200, ErrorMessage = "La domanda non può superare i 200 caratteri.")]
        public string Question { get; set; } = string.Empty;

        [Required(ErrorMessage = "La risposta è obbligatoria.")]
        [MaxLength(500, ErrorMessage = "La risposta non può superare i 500 caratteri.")]
        public string Answer { get; set; } = string.Empty;

        // Campi di sola lettura per il client (non hanno validazione in ingresso)
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedA { get; set; }

        // Info autore (opzionale, per visualizzazione)
        public string? AuthorName { get; set; }
        public string? AuthorId { get; set; }
    }
}
