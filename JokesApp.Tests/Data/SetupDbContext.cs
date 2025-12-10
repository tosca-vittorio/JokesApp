using System;
using Microsoft.EntityFrameworkCore;
using JokesApp.Server.Data;

namespace JokesApp.Tests.Data
{
    public static class SetupDbContext
    {
        public static JokesDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<JokesDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // database unico per ogni test
                .Options;

            return new JokesDbContext(options);
        }
    }
}
