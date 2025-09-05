
using System.Diagnostics;
using GameLibrary.Repositories;
using GameLibrary.Services;
using Gtk;

namespace GameLibrary;

public class GameOverview : Window
{
  public GameOverview()
  {
    var header = HeaderBar.New();
    var title = Label.New("Your Games");
    title.SetXalign(0f);
    header.SetTitleWidget(title);
    header.ShowTitleButtons = true;
    Titlebar = header;

    SetDefaultSize(370, 800);

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


  public GameList() : base()
  {
    SetOrientation(Orientation.Vertical);
    SetSpacing(5);

    var favGamesExpander = Gtk.Expander.New("Favorites");
    var favGamesBox = BuildGameList(_gameRepository.GetFavoritesGameList());
    favGamesExpander.SetExpanded(true);
    favGamesExpander.AddCssClass("section-header");
    favGamesExpander.SetChild(favGamesBox);

    var allGamesExpander = Gtk.Expander.New("All Games");
    var allGamesBox = BuildGameList(_gameRepository.GetCompleteGameList());
    allGamesExpander.SetExpanded(true);
    allGamesExpander.AddCssClass("section-header");
    allGamesExpander.SetChild(allGamesBox);

    Append(favGamesExpander);
    Append(allGamesExpander);
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

      textBox.Append(nameLabel);
      textBox.Append(playedLabel);

      var installedLabel = Label.New(game.IsInstalled ? "✅" : "❌");
      installedLabel.MarginEnd = 12;
      installedLabel.Halign = Align.End;

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

  private string SetPlayedTime(int time)
  {
    var hours = time / 60;
    var minutes = time - (hours * 60);

    return $"Play Time: {hours}h {minutes:D2}m";
  }

  private void StartGame(int id)
  {
    _steamService.StartGame(id);

    _gameRepository.UpdateLastPlayed(id);
  }
}

