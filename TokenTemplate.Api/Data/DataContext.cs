using Microsoft.EntityFrameworkCore;
using TokenTemplate.Api.Model;

namespace TokenTemplate.Api.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}