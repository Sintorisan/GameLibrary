using System.Diagnostics;
using GameLibrary;
using GameLibrary.Services;

var steamService = new SteamService();
var mostRecentUser = steamService.GetSteamUser();

var dbService = new DatabaseService();
await dbService.SetUpDatabase(mostRecentUser.SteamId);

var app = Gtk.Application.New("com.gamelauncher", Gio.ApplicationFlags.FlagsNone);

app.OnActivate += (sender, args) =>
{
  var cssProvider = Gtk.CssProvider.New();
  cssProvider.LoadFromPath("Client/styles.css");

  Gtk.StyleContext.AddProviderForDisplay(
      Gdk.Display.GetDefault(),
      cssProvider,
      800
  );

  // Window setup
  var window = new GameOverview(mostRecentUser.PersonaName);
  window.Application = (Gtk.Application)sender;
  window.Show();
};

return app.RunWithSynchronizationContext(null);
