using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), ISqlUnitOfWork
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}
