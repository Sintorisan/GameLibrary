using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameLibrary;

public class ApiService
{
  private readonly HttpClient _httpClient;
  private string _apiKey;
  private string _steamId = "76561198063612372";

  public ApiService()
  {
    _apiKey = Environment.GetEnvironmentVariable("STEAM_API_KEY") ?? string.Empty;
    _httpClient = new HttpClient();
  }

  public async Task<List<Game>> GetGamesAsync()
  {
    var respone = await _httpClient.GetAsync($"http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={_apiKey}&steamid={_steamId}&include_appinfo=true&include_played_free_games=true&format=json");
    respone.EnsureSuccessStatusCode();

    var json = await respone.Content.ReadAsStringAsync();

    var result = JsonSerializer.Deserialize<SteamResult>(json);

    return result?.Games ?? new();
  }
}

public class SteamResponse
{
  [JsonPropertyName("response")]
  public SteamResult Response { get; set; } = new();
}

public class SteamResult
{
  [JsonPropertyName("games")]
  public List<Game> Games { get; set; } = new();
}
