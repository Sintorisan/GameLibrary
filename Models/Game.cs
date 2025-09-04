using System.Text.Json.Serialization;

namespace GameLibrary;

public class Game
{
  [JsonPropertyName("appId")]
  public int Id { get; set; }

  [JsonPropertyName("name")]
  public string Name { get; set; } = string.Empty;

  [JsonPropertyName("playtime_forever")]
  public int PlayTime { get; set; }

  [JsonPropertyName("img_icon_url")]
  public string IconHash { get; set; } = string.Empty;

  public bool isFavorit { get; set; }
  public DateTime LastPlayed { get; set; }
  public string IconUrl => string.IsNullOrEmpty(IconHash)
    ? string.Empty
    : $"http://media.steampowered.com/steamcommunity/public/images/apps/{Id}/{IconHash}.jpg";
}
