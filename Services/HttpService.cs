using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameLibrary.Services;

public class HttpService
{
  private readonly HttpClient _httpClient;
  private string _apiKey;
  private string _steamId = "76561198063612372";

  public HttpService()
  {
    _apiKey = Environment.GetEnvironmentVariable("STEAM_API_KEY") ?? string.Empty;
    _httpClient = new HttpClient();
  }

  public async Task<List<Game>> GetGamesAsync()
  {
    var respone = await _httpClient.GetAsync($"http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={_apiKey}&steamid={_steamId}&include_appinfo=true&include_played_free_games=true&format=json");
    respone.EnsureSuccessStatusCode();

    var json = await respone.Content.ReadAsStringAsync();

    var result = JsonSerializer.Deserialize<SteamResponse>(json);

    return result?.Response.Games ?? new();
  }


  public async Task DownloadImageAsync(Game game)
  {
    var url = $"http://media.steampowered.com/steamcommunity/public/images/apps/{game.Id}/{game.IconHash}.jpg";

    using var response = await _httpClient.GetAsync(url);
    response.EnsureSuccessStatusCode();

    await using var stream = await response.Content.ReadAsStreamAsync();
    await using var fs = new FileStream(game.IconPath, FileMode.Create);
    await stream.CopyToAsync(fs);
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
