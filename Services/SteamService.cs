using System.Diagnostics;

namespace GameLibrary.Services;

public class SteamService
{
  private readonly string _steamPath = @"C:\Program Files (x86)\Steam";

  public bool IsGameInstalled(int appId)
  {
    // Step 1: Find all library folders
    var libraryFile = Path.Combine(_steamPath, "steamapps", "libraryfolders.vdf");
    if (!File.Exists(libraryFile)) return false;

    var libraryPaths = ParseLibraryFolders(libraryFile);

    // Step 2: Look for manifest in each library
    foreach (var library in libraryPaths)
    {
      var manifest = Path.Combine(library, "steamapps", $"appmanifest_{appId}.acf");
      if (File.Exists(manifest))
        return true;
    }

    return false;
  }

  private List<string> ParseLibraryFolders(string vdfPath)
  {
    var lines = File.ReadAllLines(vdfPath);
    var paths = new List<string>();

    foreach (var line in lines)
    {
      if (line.Contains(":\\\\"))
      {
        var path = line.Split('"')[3].Replace(@"\\", @"\");
        paths.Add(path);
      }
    }

    return paths;
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
}