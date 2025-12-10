using Xunit;

namespace JokesApp.Test
{
    public class ExampleTests2
    {
        [Fact]
        public void Sum_ShouldReturnCorrectValue()
        {
            // Arrange
            int a = 2;
            int b = 3;

            // Act
            int result = a + b;

            // Assert
            Assert.Equal(5, result);
        }
    }
}
