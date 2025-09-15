
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

    public async Task SetUpDatabase(string steamid)
    {
        using var db = new AppData();
        db.Database.EnsureCreated();

        await UpdateDatabase(db, steamid);

    }

    private async Task UpdateDatabase(AppData db, string steamid)
    {
        var latestData = await _httpService.GetGamesAsync(steamid);

        if (latestData.Count <= 0)
        {
            return;
        }

        foreach (var game in latestData)
        {
            if (db.Games.Count() <= 0)
            {
                await _httpService.DownloadImageAsync(game);
                game.IsInstalled = _steamService.IsGameInstalled(game.Id);
                await db.AddAsync(game);
                await db.SaveChangesAsync();
                continue;
            }

            var g = await db.Games.FirstOrDefaultAsync(g => g.Id == game.Id) ?? null;
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
