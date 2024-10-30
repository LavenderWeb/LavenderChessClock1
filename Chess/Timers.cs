using System.Diagnostics;
using System.Timers;

namespace LavenderChessClock1.Chess
{
    public class PlayerClock
    {
        private bool firstRun = true;
        public decimal _initialTime { get; private set; }
        public decimal _fullTime { get; private set; }
        public IIncrement _increment { get; private set; }
        public decimal _timeLeft { get; private set; }
        public bool _paused { get; private set; }

        public delegate void TimeLeftUpdated(decimal timeLeft);
        public event TimeLeftUpdated? OnTimeLeftUpdated;

        public Stopwatch _stopwatch { get; set; }
        public PlayerClock(decimal initialTime, IIncrement? increment = null)
        {
            _initialTime = initialTime;
            _increment = increment ?? new NoIncrement();
            _timeLeft = _fullTime = initialTime;

            _stopwatch = new Stopwatch();
        }

        public async Task Start()
        {
            if (!firstRun)
            {
                _increment.ApplyIncrement(this);

                while (_increment._activeState)
                {
                }
            }

            _paused = false;
            _stopwatch.Start();
            await TimeLeftLoop();
            firstRun = false;
        }

        public async Task TimeLeftLoop()
        {
            while (!_paused)
            {
                await Task.Delay(10);
                UpdateTimeLeft();
            }
        }

        public void Stop()
        {
            _stopwatch.Stop();
            _paused = true;
            if (_increment is DelayIncrement delayIncrement)
            {
                delayIncrement.Reset();
            }
        }

        public void Reset()
        {
            _stopwatch?.Reset();
            _timeLeft = _fullTime = _initialTime;
            firstRun = true;
            InvokeTimeLeftUpdated(_timeLeft);
        }

        public async void Resume()
        {
            if (_increment._activeState)
            {
                _increment.ApplyIncrement(this);
            }

            _paused = false;
            _stopwatch.Start();
            await TimeLeftLoop();
            firstRun = false;
        }

        public void AddSeconds(decimal seconds)
        {
            _fullTime += seconds;
        }

        public void UpdateTimeLeft()
        {
            _timeLeft = _fullTime - (decimal)_stopwatch.Elapsed.TotalSeconds;

            if (_timeLeft <= 0)
            {
                Stop();
            }

            InvokeTimeLeftUpdated(_timeLeft);
        }

        private void InvokeTimeLeftUpdated(decimal timeLeft)
        {
            OnTimeLeftUpdated?.Invoke(timeLeft);
        }
    }
}
