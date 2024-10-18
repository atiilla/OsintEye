using GitRecon.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<EmailAuthor> EmailAuthors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Connection string to your database
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=osintdb;Trusted_Connection=True;");
    }
}
