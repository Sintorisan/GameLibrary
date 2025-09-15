using System.Diagnostics;
using VdfParser;
using VdfParser.Models;


namespace GameLibrary.Services;

public class SteamService
{
  private readonly string _steamPath = @"C:\Program Files (x86)\Steam";

  public bool IsGameInstalled(int appId)
  {
    // Step 1: Find all library folders
    var libraryFile = Path.Combine(_steamPath, "steamapps", "libraryfolders.vdf");
    if (!File.Exists(libraryFile))
    {
      return false;
    }

    var libraries = new VdfFileParser().ParseLibraryData(libraryFile);

    // Step 2: Look for manifest in each library

    foreach (var library in libraries)
    {
      var manifest = Path.Combine(library.Path, "steamapps", $"appmanifest_{appId}.acf");
      if (File.Exists(manifest))
      {
        return true;
      }
    }

    return false;
  }

  public void EnsureSteamRunning()
  {
    if (!IsSteamRunning())
    {
      var psi = new ProcessStartInfo
      {
        FileName = "C:\\Program Files (x86)\\Steam\\steam.exe",
        Arguments = "-silent",
        UseShellExecute = true
      };

      Process.Start(psi);
    }
  }

  private bool IsSteamRunning()
  {
    var processes = Process.GetProcessesByName("steam");
    return processes.Length > 0;
  }

  public void StartGame(int id)
  {
    var psi = new ProcessStartInfo
    {
      FileName = $"steam://run/{id}",
      UseShellExecute = true
    };

    Process.Start(psi);
  }

  public SteamUser GetSteamUser()
  {
    EnsureSteamRunning();

    var userFiles = Path.Combine(_steamPath, "config", "loginusers.vdf");

    var users = new VdfFileParser().ParseUserData(userFiles);
    var mostRecentUser = users.FirstOrDefault(u => u.MostRecent == true);

    return mostRecentUser ?? new SteamUser();
  }
}

