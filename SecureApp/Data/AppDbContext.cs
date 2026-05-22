using Microsoft.EntityFrameworkCore;
using SecureApp.Models;

namespace SecureApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
}