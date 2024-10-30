using System.Diagnostics;
using System.Timers;

namespace LavenderChessClock1.Chess
{
    public class PlayerClock
    {
        private bool _firstRun = true;
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
            if (!_firstRun)
            {
                Increment.ApplyIncrement(this);

                while (Increment.ActiveState)
                {
                }
            }

            Paused = false;
            Stopwatch.Start();
            await TimeLeftLoop();
            _firstRun = false;
        }

        public async Task TimeLeftLoop()
        {
            while (!Paused)
            {
                await Task.Delay(10);
                UpdateTimeLeft();
            }
        }

        public void Stop()
        {
            Stopwatch.Stop();
            Paused = true;
            if (Increment is DelayIncrement delayIncrement)
            {
                delayIncrement.Reset();
            }
        }

        public void Reset()
        {
            Stopwatch?.Reset();
            TimeLeft = FullTime = InitialTime;
            _firstRun = true;
            InvokeTimeLeftUpdated(TimeLeft);
        }

        public async void Resume()
        {
            if (Increment.ActiveState)
            {
                Increment.ApplyIncrement(this);
            }

            Paused = false;
            Stopwatch.Start();
            await TimeLeftLoop();
            _firstRun = false;
        }

        public void AddSeconds(decimal seconds)
        {
            FullTime += seconds;
        }

        public void UpdateTimeLeft()
        {
            TimeLeft = FullTime - (decimal)Stopwatch.Elapsed.TotalSeconds;

            if (TimeLeft <= 0)
            {
                Stop();
            }

            InvokeTimeLeftUpdated(TimeLeft);
        }

        private void InvokeTimeLeftUpdated(decimal timeLeft)
        {
            OnTimeLeftUpdated?.Invoke(timeLeft);
        }
    }
}
