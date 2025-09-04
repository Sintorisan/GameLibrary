
using Microsoft.EntityFrameworkCore;

namespace GameLibrary;

public class DatabaseService
{
    private readonly ApiService _apiService;

    public DatabaseService()
    {
        _apiService = new();
    }

    public async Task SetUpDatabase()
    {
        using var db = new AppData();
        db.Database.EnsureCreated();

        try
        {
            await UpdateDatabase(db);
        }
        catch { }
    }

    private async Task UpdateDatabase(AppData db)
    {
        var latestData = await _apiService.GetGamesAsync();

        foreach (var game in latestData)
        {
            var g = await db.Games.FirstOrDefaultAsync(g => g.Id == game.Id);

            if (g == null)
            {
                await db.AddAsync(game);
            }
            else
            {
                g.PlayTime = game.PlayTime;
            }
        }

        await db.SaveChangesAsync();
    }
}
