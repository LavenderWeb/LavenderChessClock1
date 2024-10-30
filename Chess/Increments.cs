using System.Diagnostics;

namespace LavenderChessClock1.Chess
{
    public interface IIncrement
    {
        public bool _activeState { get; set; }
        public int _incrementSeconds { get; set; }
        void ApplyIncrement(PlayerClock clock);
    }

    public interface IDelayed
    {
        public decimal DelayLeft { get; set; }
        void PauseDelay();
    }

    public class NoIncrement : IIncrement
    {
        public bool _activeState { get; set; } = false;
        public int _incrementSeconds { get; set; } = 0;
        public void ApplyIncrement(PlayerClock clock) { return; }
    }

    public class AdditionIncrement : IIncrement
    {
        public bool _activeState { get; set; } = false;
        public int _incrementSeconds { get; set; }
        public AdditionIncrement(int incrementTime)
        {
            _incrementSeconds = incrementTime;
        }

        public void ApplyIncrement(PlayerClock clock)
        {
            _activeState = true;
            clock.AddSeconds((decimal)_incrementSeconds);
            _activeState = false;
        }
        
    }

    public class DelayIncrement : IIncrement, IDelayed
    {
        public bool _activeState { get; set; } = false;
        public int _incrementSeconds { get; set; }

        public Stopwatch _delayStopwatch { get; set; }
        public decimal DelayLeft { get; set; }

        public delegate void CountdownTick();
        public event CountdownTick OnCountDownTick;

        public DelayIncrement(int incrementTime)
        {
            _incrementSeconds = incrementTime;
            DelayLeft = _incrementSeconds;

            _delayStopwatch = new Stopwatch();
        }

        public async void ApplyIncrement(PlayerClock clock)
        {
            _activeState = true;
            _delayStopwatch.Start();
            await TimeLeftLoop();
        }

        public async Task TimeLeftLoop()
        {
            while (_activeState && _delayStopwatch.IsRunning)
            {
                _delayStopwatch.Start();
                DelayLeft = _incrementSeconds - (decimal)_delayStopwatch.Elapsed.TotalSeconds; ;
                await SetDelayLeft();
            }
        }

        public async Task SetDelayLeft()
        {   
            if (DelayLeft <= 0)
            {
                _delayStopwatch.Reset();
                DelayLeft = _incrementSeconds;
                _activeState = false;
            }
            else
            {
                DelayLeft = _incrementSeconds - (decimal)_delayStopwatch.Elapsed.TotalSeconds; ;
            }
        }

        public void Reset()
        {
            _delayStopwatch.Reset();
            DelayLeft = _incrementSeconds;
            _activeState = false;
        }

        public void PauseDelay()
        {
            _delayStopwatch.Stop();
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
