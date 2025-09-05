
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GameLibrary.Repositories;

public class GameRepository
{
  public List<Game> GetCompleteGameList()
  {
    using var db = new AppData();
    var games = db.Games.OrderBy(g => g.Name).ToList();

    return games;
  }

  public List<Game> GetFavoritesGameList()
  {
    using var db = new AppData();
    var games = db.Games.Where(g => g.IsFavorit == true).OrderBy(g => g.LastPlayed).ToList();

    return games;
  }

  public void UpdateLastPlayed(int id)
  {
    using var db = new AppData();
    var game = db.Games.FirstOrDefault(g => g.Id == id);

    game.LastPlayed = DateTime.UtcNow;
    db.SaveChanges();
  }

  public void UpdateIsFavorite(int id)
  {
    using var db = new AppData();
    var game = db.Games.FirstOrDefault(g => g.Id == id);

    game.IsFavorit = !game.IsFavorit;
    db.SaveChanges();
  }
}
