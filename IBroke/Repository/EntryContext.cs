using IBroke.Models;
using Microsoft.EntityFrameworkCore;

namespace IBroke.Repository;

public class EntryContext : DbContext
{
    public DbSet<Entry> Entries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=expenses.db");
    }
}
