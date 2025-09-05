using System.Diagnostics;
using GameLibrary;
using GameLibrary.Services;

var dbService = new DatabaseService();
await dbService.SetUpDatabase();

var steamService = new SteamService();
steamService.EnsureSteamRunning();

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
  var window = new GameOverview();
  window.Application = (Gtk.Application)sender;
  window.Show();
};

return app.RunWithSynchronizationContext(null);
