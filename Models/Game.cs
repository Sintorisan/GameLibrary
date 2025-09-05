using System.Text.Json.Serialization;

namespace GameLibrary;

public class Game
{
  [JsonPropertyName("appid")]
  public int Id { get; set; }

  [JsonPropertyName("name")]
  public string Name { get; set; } = string.Empty;

  [JsonPropertyName("playtime_forever")]
  public int PlayTime { get; set; }

  [JsonPropertyName("img_icon_url")]
  public string IconHash { get; set; } = string.Empty;
  public bool IsInstalled { get; set; }
  public bool IsFavorit { get; set; }
  public DateTime LastPlayed { get; set; }
  public string IconPath => string.IsNullOrEmpty(IconHash)
    ? string.Empty
    : $"Client/Icons/{Name}.jpg";
}
