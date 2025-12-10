using System.ComponentModel.DataAnnotations;

namespace JokesApp.Server.DTOs
{
    // DTO per visualizzare i dettagli utente
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName {  get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
