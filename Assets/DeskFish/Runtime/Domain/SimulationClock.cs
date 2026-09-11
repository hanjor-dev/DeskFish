using System;

namespace DeskFish.Runtime.Domain
{
    public sealed class SimulationClock
    {
        public DateTime LastObservedUtc { get; private set; }

        public SimulationClock(DateTime initialUtc)
        {
            LastObservedUtc = initialUtc.ToUniversalTime();
        }

        public TimeSpan AdvanceTo(DateTime utcNow, TimeSpan maximumAdvance)
        {
            var now = utcNow.ToUniversalTime();
            if (now <= LastObservedUtc) return TimeSpan.Zero;

            var elapsed = now - LastObservedUtc;
            if (elapsed > maximumAdvance) elapsed = maximumAdvance;
            LastObservedUtc = now;
            return elapsed;
        }
    }
}
