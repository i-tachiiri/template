using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class DbContextTests
{
    [Fact]
    public void CanAddTodoItem()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("test")
            .Options;

        using var context = new AppDbContext(options);
        context.TodoItems.Add(new Domain.TodoItem { Title = "test" });
        context.SaveChanges();

        Assert.Single(context.TodoItems);
    }
}
