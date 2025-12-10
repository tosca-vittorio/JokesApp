using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace JokesApp.Server.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class CustomEmailAttribute : ValidationAttribute
    {
        // Regex: consente lettere, numeri, punti, trattini, underscore nella local part
        // dominio: lettere, numeri, trattini e punti, TLD minimo 2 caratteri
        private static readonly Regex EmailRegex = new Regex(
            @"^[A-Za-z0-9._%+-]+@([A-Za-z0-9]+(-[A-Za-z0-9]+)*\.)+[A-Za-z]{2,}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant
        );

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                // Required è già gestito da [Required]
                return ValidationResult.Success;
            }

            string email = value.ToString()!.Trim();

            if (!EmailRegex.IsMatch(email))
            {
                return new ValidationResult(
                    ErrorMessage ?? "L'indirizzo e-mail non è valido."
                );
            }

            return ValidationResult.Success;
        }

        public static bool IsValidStatic(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email.Trim());
        }

    }
}
