using System.Diagnostics;

namespace LavenderChessClock1.Chess
{
    public interface IIncrement
    {
        public bool ActiveState { get; set; }
        public int IncrementSeconds { get; set; }
        void ApplyIncrement(PlayerClock clock);
    }

    public interface IDelayed
    {
        public decimal DelayLeft { get; set; }
        void PauseDelay();
    }

    public class NoIncrement : IIncrement
    {
        public bool ActiveState { get; set; } = false;
        public int IncrementSeconds { get; set; } = 0;
        public void ApplyIncrement(PlayerClock clock) { return; }
    }

    public class AdditionIncrement : IIncrement
    {
        public bool ActiveState { get; set; } = false;
        public int IncrementSeconds { get; set; }
        public AdditionIncrement(int incrementTime)
        {
            IncrementSeconds = incrementTime;
        }

        public void ApplyIncrement(PlayerClock clock)
        {
            ActiveState = true;
            clock.AddSeconds((decimal)IncrementSeconds);
            ActiveState = false;
        }
        
    }

    public class DelayIncrement : IIncrement, IDelayed
    {
        public bool ActiveState { get; set; } = false;
        public int IncrementSeconds { get; set; }

        public System.Timers.Timer DelayTimer { get; set; }
        public decimal DelayLeft { get; set; }

        public delegate void CountdownTick();
        public event CountdownTick OnCountDownTick;

        public DelayIncrement(int incrementTime)
        {
            IncrementSeconds = incrementTime;
            DelayLeft = IncrementSeconds;

            DelayTimer = new System.Timers.Timer();
        }

        public async void ApplyIncrement(PlayerClock clock)
        {
        }

        public void PauseDelay()
        {
            //throw new NotImplementedException();
        }

        public void Reset()
        {

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
