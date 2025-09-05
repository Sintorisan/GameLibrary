
using Microsoft.EntityFrameworkCore;

namespace GameLibrary.Services;

public class DatabaseService
{
    private readonly HttpService _httpService;
    private readonly SteamService _steamService;

    public DatabaseService()
    {
        _httpService = new();
        _steamService = new();
    }

    public async Task SetUpDatabase()
    {
        using var db = new AppData();
        db.Database.EnsureCreated();

        await UpdateDatabase(db);

    }

    private async Task UpdateDatabase(AppData db)
    {
        var latestData = await _httpService.GetGamesAsync();

        if (latestData.Count <= 0)
        {
            return;
        }

        foreach (var game in latestData)
        {
            var g = await db.Games.FirstOrDefaultAsync(g => g.Id == game.Id);

            if (g == null)
            {
                await _httpService.DownloadImageAsync(game);
                game.IsInstalled = _steamService.IsGameInstalled(game.Id);
                await db.AddAsync(game);
            }
            else
            {
                g.PlayTime = game.PlayTime;
                g.IsInstalled = _steamService.IsGameInstalled(g.Id);
            }
        }

        await db.SaveChangesAsync();
    }
}
