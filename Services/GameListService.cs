using LavenderChessClock1.Chess;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LavenderChessClock1.Services
{
    public interface IGamesListService
    {
        public Queue<GameModel> GameQueue { get; set; }
        public Task PopulateGamesList();
        public Task AddGameToList(GameModel game);
        public Task RemoveGameFromList(GameModel game);
        public Task<GameModel?> GetMostRecentGame();
        public Task ClearGamesList();
        public Task InvokeGamesListLoaded();

        public delegate void ListLoaded();
        public event ListLoaded? OnListLoaded;
    }

    public class LocalStorageGamesListService : IGamesListService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string StorageKey = "ChessClockGameQueue";
        private int MaxNumGames = 10;
        public delegate void ListLoaded();
        public event IGamesListService.ListLoaded? OnListLoaded;

        public Queue<GameModel> GameQueue { get; set; }

        public LocalStorageGamesListService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            GameQueue = new Queue<GameModel>(10);
        }

        public async Task PopulateGamesList()
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);
            try
            {
                if (!string.IsNullOrEmpty(json))
                {
                    var gameList = JsonSerializer.Deserialize<List<GameModel>>(json);
                    if (gameList != null)
                    {
                        GameQueue = new Queue<GameModel>(gameList);
                    }
                }
            }
            catch(Exception ex)
            {
                await RemoveListFromStorage();
            }

            await InvokeGamesListLoaded();
        }

        public async Task AddGameToList(GameModel game)
        {
            GameQueue.Enqueue(game);

            if (GameQueue.Count > MaxNumGames)
            {
                GameQueue.Dequeue();
            }

            await SaveListToStorage();
        }

        public async Task RemoveGameFromList(GameModel game)
        {
            if (GameQueue.Count > 0)
            {
                GameQueue.Dequeue();
                await SaveListToStorage();
            }
        }

        public async Task<GameModel?> GetMostRecentGame()
        {
            if (GameQueue.Count == 0)
            {
                return null;
            }
            else if (GameQueue.Count == 1)
            {
                return GameQueue.Peek();
            }

            return GameQueue.Last();
        }

        public async Task ClearGamesList()
        {
            GameQueue.Clear();
            await RemoveListFromStorage();
        }

        private async Task SaveListToStorage()
        {
            var gameList = GameQueue.ToList(); // Convert Queue to List for serialization
            var json = JsonSerializer.Serialize(gameList);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
        }

        private async Task RemoveListFromStorage()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        }

        public async Task InvokeGamesListLoaded()
        {
            OnListLoaded?.Invoke();
        }
    }
}
