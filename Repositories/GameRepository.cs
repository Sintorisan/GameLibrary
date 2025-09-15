namespace GameLibrary.Repositories;

public class GameRepository
{
  public List<Game> GetCompleteGameList()
  {
    using var db = new AppData();
    var games = db.Games
    .OrderBy(g => !g.IsInstalled)
    .ThenByDescending(g => g.LastPlayed)
    .ThenBy(g => g.Name)
    .ToList();

    return games;
  }

  public List<Game> GetFavoritesGameList()
  {
    using var db = new AppData();
    var games = db.Games
    .Where(g => g.IsFavorit == true)
    .OrderBy(g => g.LastPlayed)
    .ThenBy(g => g.IsInstalled)
    .ToList();

    return games;
  }

  public void UpdateLastPlayed(int id)
  {
    using var db = new AppData();
    var game = db.Games.FirstOrDefault(g => g.Id == id);

    if (game == null || game.IsInstalled == false)
    {
      return;
    }

    game.LastPlayed = DateTime.UtcNow;
    db.SaveChanges();
  }

  public void UpdateIsFavorite(int id)
  {
    using var db = new AppData();
    var game = db.Games.FirstOrDefault(g => g.Id == id);

    if (game == null)
    {
      return;
    }

    game.IsFavorit = !game.IsFavorit;
    db.SaveChanges();
  }

  internal List<Game> GetLastWeekGameList()
  {
    throw new NotImplementedException();
  }
}
