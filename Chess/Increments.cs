using System.Diagnostics;
using System.Timers;

namespace LavenderChessClock1.Chess
{
    public interface IIncrement
    {
        public string Name { get; set; }
        public bool ActiveState { get; set; }
        public int IncrementSeconds { get; set; }
        Task ApplyIncrement(PlayerClock clock);
    }

    public interface IDelayed
    {
        public decimal DelayLeft { get; set; }
        Task PauseDelay();
        Task ResumeDelay();
        Task ResetDelay();
    }

    public class NoIncrement : IIncrement
    {
        public string Name { get; set; } = string.Empty;
        public bool ActiveState { get; set; } = false;
        public int IncrementSeconds { get; set; } = 0;
        public async Task ApplyIncrement(PlayerClock clock) { return; }
    }

    public class AdditionIncrement : IIncrement
    {
        public string Name { get; set; } = "Addition";
        public bool ActiveState { get; set; } = false;
        public int IncrementSeconds { get; set; }
        public AdditionIncrement(int incrementTime)
        {
            IncrementSeconds = incrementTime;
        }

        public async Task ApplyIncrement(PlayerClock clock)
        {
            ActiveState = true;
            await clock.AddSeconds((decimal)IncrementSeconds);
            ActiveState = false;
        }
    }

    public class DelayIncrement : IIncrement, IDelayed
    {
        public string Name { get; set; } = "Delay";
        public bool ActiveState { get; set; } = false;
        public int IncrementSeconds { get; set; }
        public System.Timers.Timer DelayTimer { get; set; }
        public decimal DelayLeft { get; set; }

        public delegate void DelayTick();
        public event DelayTick? OnDelayTick;

        public DelayIncrement(int incrementTime)
        {
            IncrementSeconds = incrementTime;
            DelayLeft = IncrementSeconds;

            DelayTimer = new System.Timers.Timer(1000);
            DelayTimer.Elapsed += DecrementDelayLeft;
        }

        public async Task ApplyIncrement(PlayerClock clock)
        {
            ActiveState = true;
            DelayLeft = IncrementSeconds;
            DelayTimer.Start();
            while (DelayLeft > 0){
                await Task.Delay(10);
            }
            await ResetDelay();
        }
        public async Task PauseDelay()
        {
            DelayTimer.Enabled = false;
        }
        public async Task ResetDelay(){
            DelayTimer.Stop();
            DelayLeft = 0;
            ActiveState = false;
        }
        public async Task ResumeDelay()
        {
            DelayTimer.Enabled = true;
        }
        private void DecrementDelayLeft(object? sender, ElapsedEventArgs e)
        {
            DelayLeft--;
            OnDelayTick.Invoke();
        }
    }

    public enum IncrementType
    {
        NoIncrement = 0,
        AdditionIncrement = 1,
        DelayIncrement = 2
    }

    public static class IncrementFactory 
    {
        public static IIncrement CreateIncrement(IncrementType incrementType, int incrementLength)
        {
            switch (incrementType)
            {
                case IncrementType.AdditionIncrement:
                    return new AdditionIncrement(incrementLength);
                case IncrementType.DelayIncrement:
                    return new DelayIncrement(incrementLength);
                default:
                case IncrementType.NoIncrement:
                    return new NoIncrement();
            }
        }
    }

}
