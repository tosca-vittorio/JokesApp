using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using JokesApp.Server.Domain.Attributes;
using JokesApp.Server.Domain.Errors;



namespace JokesApp.Server.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        // Nome visuale dell'utente, inizializzato a string.Empty per evitare null.
        private string _displayName = string.Empty;

        [MaxLength(50, ErrorMessage = ApplicationUserErrorMessages.DisplayNameMaxLength)]
        public string DisplayName
        {
            get => _displayName;
            private set
            {
                if (_displayName != value?.Trim())
                {
                    _displayName = value?.Trim() ?? string.Empty;
                    UpdatedAt = DateTime.UtcNow; // Aggiornamento automatico
                }
            }
        }

        // URL dell'immagine profilo dell'utente.
        // Opzionale, può essere null.
        private string? _avatarUrl;

        [MaxLength(2048, ErrorMessage = ApplicationUserErrorMessages.AvatarUrlMaxLength)]
        [Url(ErrorMessage = ApplicationUserErrorMessages.AvatarUrlInvalid)]
        public string? AvatarUrl
        {
            get => _avatarUrl;
            private set
            {
                if (_avatarUrl != value)
                {
                    _avatarUrl = value;
                    UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        // Data di creazione dell'account (UTC).
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // Data di ultima modifica del profilo (UTC).
        public DateTime? UpdatedAt { get; set; }

        [Required(ErrorMessage = ApplicationUserErrorMessages.EmailRequired)]
        [CustomEmail(ErrorMessage = ApplicationUserErrorMessages.EmailInvalid)]
        [MaxLength(256)]
        public override string? Email
        {
            get => base.Email;
            set
            {
                var trimmed = value?.Trim() ?? "";

                // 1) Non può essere vuota → errore Required
                if (string.IsNullOrWhiteSpace(trimmed))
                    throw new ArgumentException(ApplicationUserErrorMessages.EmailInvalid);

                // 2) Non può contenere spazi interni
                if (trimmed.Contains(" "))
                    throw new ArgumentException(ApplicationUserErrorMessages.EmailInvalid);

                // 3) Lunghezza massima
                if (trimmed.Length > 256)
                    throw new ArgumentException(ApplicationUserErrorMessages.EmailTooLong);

                // 4) Validazione formato tramite la regex statica
                if (!CustomEmailAttribute.IsValidStatic(trimmed))
                    throw new ArgumentException(ApplicationUserErrorMessages.EmailInvalid);

                // Se tutto ok, applica
                base.Email = trimmed;
                UpdatedAt = DateTime.UtcNow;
            }
        }


        private void ValidateDisplayName(string? displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException(ApplicationUserErrorMessages.DisplayNameRequired, nameof(displayName));

            if (displayName.Length > 50)
                throw new ArgumentException(ApplicationUserErrorMessages.DisplayNameMaxLength, nameof(displayName));
        }

        private static void ValidateAvatarUrl(string? avatarUrl)
        {
            if (avatarUrl is null)
                return; // opzionale

            if (avatarUrl.Length > 2048)
                throw new ArgumentException(ApplicationUserErrorMessages.AvatarUrlMaxLength, nameof(avatarUrl));

            // Se vuoi, puoi anche aggiungere un controllo semantico:
            // if (!Uri.IsWellFormedUriString(avatarUrl, UriKind.Absolute))
            //    throw new ArgumentException(ApplicationUserErrorMessages.AvatarUrlInvalid, nameof(avatarUrl));
        }

        public void UpdateProfile(
        string displayName,
        string? avatarUrl = null,
        string? email = null)
        {
            // Normalizzazione
            var name = displayName?.Trim();
            var avatar = avatarUrl?.Trim();
            var mail = email?.Trim();

            // Validazione dominio
            ValidateDisplayName(name);
            ValidateAvatarUrl(avatar);

            // Applicazione cambiamenti
            DisplayName = name!;
            AvatarUrl = avatar;

            if (mail is not null)
                Email = mail; // Il setter valida già tutto

            UpdatedAt = DateTime.UtcNow;
        }


        public void ChangeEmail(string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
                throw new ArgumentException(ApplicationUserErrorMessages.EmailRequired);

            Email = newEmail.Trim(); // Il setter fa tutta la validazione

            UpdatedAt = DateTime.UtcNow;
        }


        public void SetAvatar(string? avatarUrl)
        {
            var avatar = avatarUrl?.Trim();
            ValidateAvatarUrl(avatar);
            AvatarUrl = avatar;
            UpdatedAt = DateTime.UtcNow;
        }



        [JsonIgnore]
        public override string? PasswordHash { get; set; }

        [JsonIgnore]
        public override string? SecurityStamp { get; set; }

        // Collezione di barzellette associate all'utente:
        //  - Definisce una relazione uno-a-molti tra ApplicationUser e Joke.
        public  ICollection<Joke> Jokes { get; set; } = new List<Joke>();

    }
}