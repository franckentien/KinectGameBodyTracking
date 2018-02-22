using System;
using System.Windows.Threading;

namespace BodyTracking
{
    internal class Timer
    {
        #region Variable

        /// <summary>
        ///     A timer for time left until stop count point
        /// </summary>
        private readonly DispatcherTimer _myDispatcherTimer;

        /// <summary>
        ///     use for check if timer is active for dont recreate a new timer and increment counter only if this is true
        /// </summary>
        internal bool TimerActive { get; private set; }

        /// <summary>
        ///     minute left for timer
        /// </summary>
        internal int Timerm { get; private set; }

        /// <summary>
        ///     second left for timer
        /// </summary>
        internal int Timers { get; private set; }

        /// <summary>
        ///     millisecond left for timer
        /// </summary>
        internal int Timerms { get; private set; }

        /// <summary>
        ///     active mode in sec or in ms
        /// </summary>
        private readonly string _modeTick;

        #endregion

        #region Constructor

        /// <summary>
        ///     constructor of timer class
        /// </summary>
        public Timer()
        {
            Timerm = 0;
            Timers = 0;
            _myDispatcherTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 1, 0) //1sec 
            };

            _myDispatcherTimer.Tick += Each_Tick;
            _modeTick = "second";
        }

        /// <summary>
        ///     constructor of timer class for Opacity
        /// </summary>
        public Timer(string interval)
        {
            Timerm = 0;
            Timers = 0;
            _myDispatcherTimer = new DispatcherTimer();
            _modeTick = interval;
            _myDispatcherTimer.Interval = interval == "Opacity"
                ? new TimeSpan(0, 0, 0, 0, 40)
                : new TimeSpan(0, 0, 0, 1, 0);
            _myDispatcherTimer.Tick += Each_Tick;
        }

        #endregion

        #region function

        /// <summary>
        ///     Edit time left and start a new timer.
        /// </summary>
        /// <param name="min">minute for timer</param>
        /// <param name="sec">second for timer</param>
        /// <param name="millisec">millisecond for timer</param>
        public void ActiveTimer(int min, int sec, int millisec)
        {
            Timerm = min;
            Timers = sec;
            Timerms = millisec;
            _myDispatcherTimer.Start();
            TimerActive = true;
        }

        /// <summary>
        ///     Check if timer is active
        ///     if time left == 0 StopTimer
        ///     else remove 1 second to time left
        /// </summary>
        private void Each_Tick(object o, EventArgs sender)
        {
            if (!TimerActive) return;
            switch (_modeTick)
            {
                case "Opacity":
                    Timerms--;
                    if (Timerms <= 0)
                    {
                        StopTimer();
                    }
                    break;
                default: // second
                    Timers--;
                    if (Timerm <= 0 && Timers <= 0)
                    {
                        StopTimer();
                    }
                    else if (Timers <= 0)
                    {
                        Timerm--;
                        Timers = 60;
                    }

                    break;
            }
        }

        /// <summary>
        ///     Stop Timer
        /// </summary>
        public void StopTimer()
        {
            _myDispatcherTimer.Stop();
            TimerActive = false;
        }

        #endregion
    }
}