using Brewlib.Util;

namespace BrewLib.Time
{
    public class TimeSourceExtender : TimeSource
    {
        private readonly Clock clock = new Clock();
        private readonly TimeSource timeSource;

        private readonly ValueWindow<double> errorWindow = new(180);
        private double errorAdjustment;

        public double Current => clock.Current + errorAdjustment;

        public bool Playing
        {
            get => clock.Playing;
            set
            {
                if (clock.Playing == value)
                    return;

                timeSource.Playing = value && timeSource.Seek(clock.Current);
                clock.Playing = value;
                errorWindow.Clear();
            }
        }

        public double TimeFactor
        {
            get => clock.TimeFactor;
            set
            {
                timeSource.TimeFactor = value;
                clock.TimeFactor = value;
                errorWindow.Clear();
            }
        }

        public TimeSourceExtender(TimeSource timeSource)
        {
            this.timeSource = timeSource;
        }

        public bool Seek(double time)
        {
            if (!timeSource.Seek(time))
                timeSource.Playing = false;

            errorWindow.Clear();
            errorAdjustment = 0;
            return clock.Seek(time);
        }

        public void Update()
        {
            var playing = timeSource.Playing = clock.Playing && (timeSource.Playing || timeSource.Seek(clock.Current));
            if (!playing)
                return;

            var error = clock.Current - timeSource.Current;
            errorWindow.Add(error);
            errorAdjustment = errorAdjustment * .9 - errorWindow.GetAverage() * .1;
        }
    }
}
