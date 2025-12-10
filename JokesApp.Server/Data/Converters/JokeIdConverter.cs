using JokesApp.Server.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JokesApp.Server.Data.Converters
{
    public class JokeIdConverter : ValueConverter<JokeId, int>
    {
        public JokeIdConverter()
            : base(
                jokeId => jokeId.Value,
                value => JokeId.Create(value))
        {
        }
    }
}
