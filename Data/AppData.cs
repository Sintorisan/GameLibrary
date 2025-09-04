using Microsoft.EntityFrameworkCore;

namespace GameLibrary;

public class AppData : DbContext
{
  public DbSet<Game> Games { get; set; }


  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseSqlite("Data Source=app.db");
  }
}
