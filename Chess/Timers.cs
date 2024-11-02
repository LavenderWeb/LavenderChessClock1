using System.Diagnostics;
using System.Timers;

namespace LavenderChessClock1.Chess
{
    public class PlayerClock
    {
        public decimal InitialTime { get; private set; }
        public decimal FullTime { get; private set; }
        public IIncrement Increment { get; private set; }
        public decimal TimeLeft { get; private set; }
        public bool Paused { get; private set; }

        public delegate void TimeLeftUpdated(decimal timeLeft);
        public event TimeLeftUpdated? OnTimeLeftUpdated;

        public Stopwatch Stopwatch { get; set; }
        public PlayerClock(decimal initialTime, IIncrement? increment = null)
        {
            InitialTime = initialTime;
            Increment = increment ?? new NoIncrement();
            TimeLeft = FullTime = initialTime;

            Stopwatch = new Stopwatch();
        }

        public async Task Start()
        {
            await UpdateTimeLeft();
            await Increment.ApplyIncrement(this);

            while (Increment.ActiveState)
            {
                await Task.Delay(100);
            }

            Paused = false;
            Stopwatch.Start();
            await TimeLeftLoop();
        }

        public async Task TimeLeftLoop()
        {
            while (!Paused)
            {
                await Task.Delay(10);
                await UpdateTimeLeft();
            }
        }

        public async Task Stop()
        {
            Stopwatch.Stop();
            Paused = true;
            if (Increment is IDelayed delayIncrement)
            {
                await delayIncrement.PauseDelay();
            }
        }

        public async Task Reset()
        {
            Stopwatch?.Reset();
            TimeLeft = FullTime = InitialTime;
            await InvokeTimeLeftUpdated(TimeLeft);
        }

        public async Task Resume()
        {
            if (Increment.ActiveState && Increment is IDelayed delayed)
            {
                await delayed.ResumeDelay();

                while (Increment.ActiveState)
                {
                    await UpdateTimeLeft();
                    await Task.Delay(100);
                }
            }

            Paused = false;
            Stopwatch.Start();
            await TimeLeftLoop();
        }

        public async Task AddSeconds(decimal seconds)
        {
            FullTime += seconds;
        }

        public async Task UpdateTimeLeft()
        {
            TimeLeft = FullTime - (decimal)Stopwatch.Elapsed.TotalSeconds;

            await InvokeTimeLeftUpdated(TimeLeft);

            if (TimeLeft <= 0)
            {
                await Stop();
            }
        }

        private async Task InvokeTimeLeftUpdated(decimal timeLeft)
        {
            OnTimeLeftUpdated?.Invoke(timeLeft);
        }
    }
}
