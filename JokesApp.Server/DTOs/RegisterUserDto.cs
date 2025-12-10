using System.ComponentModel.DataAnnotations;


namespace JokesApp.Server.DTOs
{
    // DTO specifico per la registrazione (input)
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "L'e-mail è obbligatoria.")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo e-mail valido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        [MinLength(6, ErrorMessage = "La password deve essere lunga almeno 6 caratteri")]
        public string Password { get; set; } = string.Empty;

        [MaxLength(50)]
        public string DisplayName { get; set; } = string.Empty;
    }
}
