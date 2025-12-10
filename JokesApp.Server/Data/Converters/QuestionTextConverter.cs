using JokesApp.Server.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JokesApp.Server.Data.Converters
{
    public class QuestionTextConverter : ValueConverter<QuestionText, string>
    {
        public QuestionTextConverter()
            : base(
                q => q.Value,
                value => QuestionText.Create(value))
        {
        }
    }
}
