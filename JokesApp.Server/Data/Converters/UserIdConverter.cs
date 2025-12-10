using JokesApp.Server.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JokesApp.Server.Data.Converters
{
    public class UserIdConverter : ValueConverter<UserId, string>
    {
        public UserIdConverter()
            : base(
                userId => userId.Value,
                value => UserId.Create(value))
        {
        }
    }
}
