using System;
using System.Timers;
using System.Threading.Tasks;

namespace LavenderChessClock1.Chess
{
    public class Game : IDisposable
    {
        public PlayerClock Player1Clock { get; }
        public PlayerClock Player2Clock { get; }
        public bool IsPlayer1Turn { get; private set; }
        public int TurnNumber { get; private set; }
        public GameState State { get; private set; }

        public Game(decimal player1Time, decimal player2Time, IIncrement? player1Increment = null, IIncrement? player2Increment = null)
        {
            Player1Clock = new PlayerClock(player1Time, player1Increment);
            Player2Clock = new PlayerClock(player2Time, player2Increment);
            IsPlayer1Turn = true;
            State = 0;

            Player1Clock.OnTimeLeftUpdated += CheckIfGameOver;
            Player2Clock.OnTimeLeftUpdated += CheckIfGameOver;
        }

        public async Task StartGame()
        {
            State = GameState.Active;
            if (IsPlayer1Turn)
                await Player1Clock.Start();
            else
                await Player2Clock.Start();
        }

        public async Task SwitchTurns()
        {
            if (State != GameState.Active)
            {
                return;
            }

            if (IsPlayer1Turn)
            {
                IsPlayer1Turn = false;
                await Player1Clock.Stop();
                await Player2Clock.Start();
            }
            else
            {
                IsPlayer1Turn = true;
                await Player2Clock.Stop();
                await Player1Clock.Start();
            }

        }

        public async Task StopGame()
        {
            await Player1Clock.Stop();
            await Player2Clock.Stop();
            State = GameState.PostGame;
        }

        public async Task PauseGame()
        {
            State = GameState.Paused;
            await Player1Clock.Stop();
            await Player2Clock.Stop();
        }

        public async Task ResetGame()
        {
            State = GameState.PreGame;
            await Player1Clock.Reset();
            await Player2Clock.Reset();
            IsPlayer1Turn = true;
        }

        public async Task ResumeGame()
        {
            State = GameState.Active;
            if (IsPlayer1Turn)
            {
                await Player1Clock.Resume();
            }
            else
            {
                await Player2Clock.Resume();
            }
        }

        public async void CheckIfGameOver(decimal timeLeft)
        {
            if (timeLeft <= 0)
            {
                await StopGame();
            }
        }

        public void Dispose()
        {
            Player1Clock.OnTimeLeftUpdated -= CheckIfGameOver;
            Player2Clock.OnTimeLeftUpdated -= CheckIfGameOver;
        }
    }

    public enum GameState
    {
        PreGame = 0,
        Active = 1,
        Paused = 2,
        PostGame = 3
    }
}
