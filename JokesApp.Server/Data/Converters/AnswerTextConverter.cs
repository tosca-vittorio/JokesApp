using JokesApp.Server.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JokesApp.Server.Data.Converters
{
    public class AnswerTextConverter : ValueConverter<AnswerText, string>
    {
        public AnswerTextConverter()
            : base(
                a => a.Value,
                value => AnswerText.Create(value))
        {
        }
    }
}
