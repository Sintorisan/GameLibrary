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
  private List<Game> _recentlyGames = new();
  private List<Game> _allGames = new();
  private Expander _favGamesExpander;
  private Expander _recentlyExpander;
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

    _recentlyExpander = Expander.New("Recently Played");
    _recentlyExpander.SetExpanded(true);
    _recentlyExpander.AddCssClass("section-header");
    _recentlyExpander.SetChild(BuildGameList(_recentlyGames));

    _allGamesExpander = Expander.New("All Games");
    _allGamesExpander.SetExpanded(false);
    _allGamesExpander.AddCssClass("section-header");
    _allGamesExpander.SetChild(BuildGameList(_allGames));

    Append(_favGamesExpander);
    Append(_recentlyExpander);
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

      var playedLabel = Label.New(SetPlayedTime(game.PlayTime));
      playedLabel.SetXalign(0f);
      playedLabel.AddCssClass("game-time");

      var lastPlayedLabel = Label.New(SetLastPlayed(game.LastPlayed));
      lastPlayedLabel.SetXalign(0f);
      lastPlayedLabel.AddCssClass("game-time");
      lastPlayedLabel.MarginBottom = 5;

      textBox.Append(nameLabel);
      textBox.Append(playedLabel);
      textBox.Append(lastPlayedLabel);

      var installedLabel = Label.New(game.IsInstalled ? "✅" : "❌");
      installedLabel.MarginEnd = 12;
      installedLabel.Halign = Align.End;

      var rowBox = Box.New(Orientation.Horizontal, 8);
      rowBox.Append(icon);
      rowBox.Append(textBox);
      rowBox.Append(installedLabel);

      var gameExpander = Expander.New(null);
      gameExpander.SetLabelWidget(rowBox);
      gameExpander.AddCssClass("game-row");
      gameExpander.SetChild(OptionsButtons(game));

      box.Append(gameExpander);
    }

    return box;
  }
  private Box OptionsButtons(Game game)
  {
    var optionsBox = Box.New(Orientation.Horizontal, 12);
    optionsBox.AddCssClass("options-box");

    if (game.IsInstalled)
    {
      var launchBtn = Button.NewWithLabel("🚀 Launch");
      launchBtn.AddCssClass("option-button");
      launchBtn.Hexpand = true;
      launchBtn.Halign = Align.Fill;
      launchBtn.OnClicked += (sender, e) => StartGame(game.Id);
      optionsBox.Append(launchBtn);

      var uninstallBtn = Button.NewWithLabel("🗑️ Uninstall");
      uninstallBtn.AddCssClass("option-button");
      uninstallBtn.Hexpand = true;
      uninstallBtn.Halign = Align.Fill;
      uninstallBtn.OnClicked += (sender, e) => UninstallGame(game.Id);
      optionsBox.Append(uninstallBtn);
    }
    else
    {
      var installBtn = Button.NewWithLabel("📥 Install");
      installBtn.AddCssClass("option-button");
      installBtn.Hexpand = true;
      installBtn.Halign = Align.Fill;
      installBtn.OnClicked += (sender, e) => InstallGame(game.Id);
      optionsBox.Append(installBtn);

    }

    var favBtn = Button.NewWithLabel(game.IsFavorit ? "💔 Remove Fav" : "⭐ Add Fav");
    favBtn.AddCssClass("option-button");
    favBtn.Hexpand = true;
    favBtn.Halign = Align.Fill;
    favBtn.OnClicked += (sender, e) => UpdateFavorites(game.Id);
    optionsBox.Append(favBtn);

    return optionsBox;
  }

  private void StartGame(int id)
  {
    _gameRepository.UpdateLastPlayed(id);
    _steamService.StartGame(id);
    RefreshUI();
  }

  private void InstallGame(int id)
  {
    _steamService.InstallGame(id);
    RefreshUI();
  }

  private void UninstallGame(int id)
  {
    _steamService.UninstallGame(id);
    RefreshUI();
  }

  private void PopulateGameLists()
  {
    _allGames = _gameRepository.GetCompleteGameList();

    _favGames = _allGames
      .Where(g => g.IsFavorit == true)
      .ToList();

    _recentlyGames = _allGames
      .OrderByDescending(g => g.LastPlayed)
      .Take(3)
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

  private void UpdateFavorites(int id)
  {
    _gameRepository.UpdateIsFavorite(id);
    RefreshUI();
  }

  private void RefreshUI()
  {
    _favGames = _gameRepository.GetFavoritesGameList();
    _allGames = _gameRepository.GetCompleteGameList();
    _recentlyGames = _gameRepository.GetRecentlyGameList();

    _favGamesExpander.SetChild(BuildGameList(_favGames));
    _recentlyExpander.SetChild(BuildGameList(_recentlyGames));
    _allGamesExpander.SetChild(BuildGameList(_allGames));
  }
}

