using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using JokesApp.Server.Data;
using JokesApp.Server.Models;
using System.Threading.Tasks;
using System.Linq;

namespace JokesApp.Tests.Data
{
    public class SetupDbContextTests
    {
        [Fact]
        public void CreateContext_ShouldReturnNewDbContextInstance()
        {
            var context1 = SetupDbContext.CreateContext();
            var context2 = SetupDbContext.CreateContext();

            context1.Should().NotBeNull();
            context2.Should().NotBeNull();
            context1.Should().NotBeSameAs(context2);
        }

        [Fact]
        public async Task CreateContext_ShouldAllowCRUDOperations()
        {
            await using var context = SetupDbContext.CreateContext();

            // CREATE
            var joke = new Joke("Domanda", "Risposta", "user123");
            await context.Jokes.AddAsync(joke);
            await context.SaveChangesAsync();

            // READ
            var saved = await context.Jokes.FirstOrDefaultAsync();
            saved.Should().NotBeNull();
            saved!.Question.Should().Be("Domanda");

            // UPDATE
            saved.Update("Domanda2", "Risposta2");
            context.Jokes.Update(saved);
            await context.SaveChangesAsync();

            var updated = await context.Jokes.FirstOrDefaultAsync();
            updated!.Question.Should().Be("Domanda2");
            updated.Answer.Should().Be("Risposta2");

            // DELETE
            context.Jokes.Remove(updated);
            await context.SaveChangesAsync();

            (await context.Jokes.CountAsync()).Should().Be(0);
        }

        [Fact]
        public async Task MultipleContexts_ShouldBeIndependent()
        {
            await using var context1 = SetupDbContext.CreateContext();
            await using var context2 = SetupDbContext.CreateContext();

            var joke1 = new Joke("Q1", "A1", "u1");
            var joke2 = new Joke("Q2", "A2", "u2");

            await context1.Jokes.AddAsync(joke1);
            await context1.SaveChangesAsync();

            await context2.Jokes.AddAsync(joke2);
            await context2.SaveChangesAsync();

            (await context1.Jokes.CountAsync()).Should().Be(1);
            (await context2.Jokes.CountAsync()).Should().Be(1);

            (await context1.Jokes.FirstOrDefaultAsync())!.Question.Should().Be("Q1");
            (await context2.Jokes.FirstOrDefaultAsync())!.Question.Should().Be("Q2");
        }
    }
}
