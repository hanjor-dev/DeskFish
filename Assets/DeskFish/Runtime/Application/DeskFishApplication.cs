using System;
using DeskFish.Runtime.Domain;

namespace DeskFish.Runtime.Application
{
    public sealed class DeskFishApplication
    {
        public const int DefaultFishCount = 3;
        public static readonly TimeSpan MaximumOfflineAdvance = TimeSpan.FromHours(24);
        public static readonly TimeSpan AdultAge = TimeSpan.FromDays(7);

        public Tank Tank { get; }
        public SimulationClock Clock { get; }
        public event Action<IDeskFishEvent> EventPublished;

        private DeskFishApplication(Tank tank, SimulationClock clock)
        {
            Tank = tank;
            Clock = clock;
        }

        public static DeskFishApplication StartNew(DateTime utcNow)
        {
            var now = utcNow.ToUniversalTime();
            var application = new DeskFishApplication(new Tank(), new SimulationClock(now));
            var species = new[]
            {
                new FishSpecies("amber_guppy", "琥珀孔雀鱼", "amber"),
                new FishSpecies("blue_tetra", "蓝灯鱼", "blue"),
                new FishSpecies("coral_platy", "珊瑚月光鱼", "coral")
            };
            for (var i = 0; i < species.Length; i++)
                application.Tank.AddFish(new Fish("fish-" + (i + 1), species[i], species[i].DisplayName, now));
            application.Publish(new SessionStarted(application.Tank));
            return application;
        }

        public TimeSpan AdvanceTo(DateTime utcNow)
        {
            var elapsed = Clock.AdvanceTo(utcNow, MaximumOfflineAdvance);
            Tank.Advance(elapsed, AdultAge);
            return elapsed;
        }

        public void Execute(IDeskFishCommand command)
        {
            if (command is ClickWater click) Publish(new WaterClicked(click.Position));
            else if (command is StartAttraction start) { Tank.SetAttraction(true); Publish(new AttractionChanged(start.Position, true)); }
            else if (command is UpdateAttraction update && Tank.Fish.Count > 0) Publish(new AttractionChanged(update.Position, true));
            else if (command is EndAttraction end) { Tank.SetAttraction(false); Publish(new AttractionChanged(default(WaterPosition), false)); }
            else throw new ArgumentNullException(nameof(command));
        }

        private void Publish(IDeskFishEvent value) { EventPublished?.Invoke(value); }
    }
}
