using Microsoft.EntityFrameworkCore;

namespace GameLibrary;

public class AppData : DbContext
{
  public DbSet<Game> Games { get; set; }


  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    var dbPath = Path.Combine(GetAppDataPath("Data"), "app.db");
    optionsBuilder.UseSqlite($"Data Source={dbPath}");
  }

  public static string GetAppDataPath(string subfolder)
  {
    var baseDir = AppContext.BaseDirectory;
    var path = Path.Combine(baseDir, "Client", subfolder);

    if (!Directory.Exists(path))
      Directory.CreateDirectory(path);

    return path;
  }

}
