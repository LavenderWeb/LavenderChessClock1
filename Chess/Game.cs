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

        public void SwitchTurns()
        {
            if (State != GameState.Active)
            {
                return;
            }

            if (IsPlayer1Turn)
            {
                Player1Clock.Stop();
                Player2Clock.Start();
            }
            else
            {
                Player2Clock.Stop();
                Player1Clock.Start();
            }

            IsPlayer1Turn = !IsPlayer1Turn;
            TurnNumber++;
        }

        public void StopGame()
        {
            Player1Clock.Stop();
            Player2Clock.Stop();
            State = GameState.PostGame;
        }

        public void PauseGame()
        {
            Player1Clock.Stop();
            Player2Clock.Stop();
            State = GameState.Paused;
        }

        public void ResetGame()
        {
            Player1Clock.Reset();
            Player2Clock.Reset();
            IsPlayer1Turn = true;
            State = GameState.PreGame;
        }

        public void ResumeGame()
        {
            if (IsPlayer1Turn)
            {
                Player1Clock.Resume();
            }
            else
            {
                Player2Clock.Resume();
            }
            State = GameState.Active;
        }

        public void CheckIfGameOver(decimal timeLeft)
        {
            if (timeLeft <= 0)
            {
                StopGame();
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
