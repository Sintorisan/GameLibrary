using GameLibrary.Repositories;
using GameLibrary.Services;
using Gtk;

namespace GameLibrary;

public class GameOverview : Window
{
  public GameOverview(string personaName)
  {
    var header = HeaderBar.New();
    var title = Label.New(personaName);
    title.SetXalign(0f);
    header.SetTitleWidget(title);
    header.ShowTitleButtons = true;
    Titlebar = header;

    SetDefaultSize(400, 800);

    var gameList = new GameList();

    var scrolledWindow = ScrolledWindow.New();
    scrolledWindow.SetChild(gameList);

    Child = scrolledWindow;
  }
}

public class GameList : Box
{
  private readonly GameRepository _gameRepository = new();
  private readonly SteamService _steamService = new();
  private List<Game> _favGames = new();
  private List<Game> _lastWeekGames = new();
  private List<Game> _allGames = new();
  private Expander _favGamesExpander;
  private Expander _lastWeekExpander;
  private Expander _allGamesExpander;

  public GameList() : base()
  {
    SetOrientation(Orientation.Vertical);
    SetSpacing(5);

    PopulateGameLists();

    _favGamesExpander = Expander.New("Favorites");
    _favGamesExpander.SetExpanded(true);
    _favGamesExpander.AddCssClass("section-header");
    _favGamesExpander.SetChild(BuildGameList(_favGames));

    _lastWeekExpander = Expander.New("Last Week Play");
    _lastWeekExpander.SetExpanded(true);
    _lastWeekExpander.AddCssClass("section-header");
    _lastWeekExpander.SetChild(BuildGameList(_lastWeekGames));

    _allGamesExpander = Expander.New("All Games");
    _allGamesExpander.SetExpanded(true);
    _allGamesExpander.AddCssClass("section-header");
    _allGamesExpander.SetChild(BuildGameList(_allGames));

    Append(_favGamesExpander);
    Append(_lastWeekExpander);
    Append(_allGamesExpander);
  }

  private Box BuildGameList(List<Game> games)
  {
    var box = Box.New(Orientation.Vertical, 3);

    foreach (var game in games)
    {
      var icon = Image.NewFromFile(game.IconPath);
      icon.SetPixelSize(32);
      icon.Halign = Align.Start;

      var textBox = Box.New(Orientation.Vertical, 2);
      textBox.Hexpand = true;
      textBox.Halign = Align.Start;

      var nameLabel = Label.New(game.Name);
      nameLabel.SetXalign(0f);
      nameLabel.AddCssClass("game-name");

      var timeBox = Box.New(Orientation.Vertical, 2);

      var lastPlayedLable = Label.New(SetLastPlayed(game.LastPlayed));
      lastPlayedLable.SetXalign(0f);
      lastPlayedLable.AddCssClass("game-time");

      var playedLabel = Label.New(SetPlayedTime(game.PlayTime));
      playedLabel.SetXalign(0f);
      playedLabel.AddCssClass("game-time");

      timeBox.Append(playedLabel);
      timeBox.Append(lastPlayedLable);

      textBox.Append(nameLabel);
      textBox.Append(timeBox);

      var installedLabel = Label.New(game.IsInstalled ? "✅" : "❌");
      installedLabel.MarginEnd = 12;
      installedLabel.Halign = Align.End;

      // TODO: Figure out a better solution on how move games to favorite
      // var favLabel = Label.New(game.IsFavorit ? "🥰" : "🥱");
      // var favButton = Button.New();
      // favButton.AddCssClass("fav-button");
      // favButton.SetChild(favLabel);

      // favButton.OnClicked += (sender, e) =>
      // {
      //   UpdateFavorites(game.Id);

      // };

      var rowBox = Box.New(Orientation.Horizontal, 8);
      rowBox.Append(icon);
      rowBox.Append(textBox);
      rowBox.Append(installedLabel);

      var button = Button.New();
      button.SetChild(rowBox);

      button.OnClicked += (sender, e) =>
      {
        StartGame(game.Id);
      };

      box.Append(button);
    }

    return box;
  }

  private void PopulateGameLists()
  {
    _allGames = _gameRepository.GetCompleteGameList();

    _favGames = _allGames.Where(g => g.IsFavorit == true)
      .ToList();

    _lastWeekGames = _allGames
      .Where(g => g.LastPlayed > DateTime.UtcNow.AddDays(-7))
      .ToList();
  }

  private string? SetLastPlayed(DateTime lastPlayed)
  {
    return lastPlayed != DateTime.MinValue ? $"Last Played: {lastPlayed:dd/MM/yyyy}" : "Not Played Yet!";
  }

  private string SetPlayedTime(int time)
  {
    var hours = time / 60;
    var minutes = time - (hours * 60);

    return $"Play Time: {hours}h {minutes:D2}m";
  }

  private void StartGame(int id)
  {
    _gameRepository.UpdateLastPlayed(id);
    _steamService.StartGame(id);
    RefreshUI();
  }

  private void UpdateFavorites(int id)
  {
    _gameRepository.UpdateIsFavorite(id);
    RefreshUI();
  }

  private void RefreshUI()
  {
    _favGames = _gameRepository.GetFavoritesGameList();
    _allGames = _gameRepository.GetCompleteGameList();

    _favGamesExpander.SetChild(BuildGameList(_favGames));
    _allGamesExpander.SetChild(BuildGameList(_allGames));
  }
}

