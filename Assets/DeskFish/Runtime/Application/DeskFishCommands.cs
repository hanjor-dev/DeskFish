using DeskFish.Runtime.Domain;

namespace DeskFish.Runtime.Application
{
    public interface IDeskFishCommand { }

    public sealed class ClickWater : IDeskFishCommand
    {
        public WaterPosition Position { get; }
        public ClickWater(WaterPosition position) { Position = position; }
    }

    public sealed class StartAttraction : IDeskFishCommand
    {
        public WaterPosition Position { get; }
        public StartAttraction(WaterPosition position) { Position = position; }
    }

    public sealed class UpdateAttraction : IDeskFishCommand
    {
        public WaterPosition Position { get; }
        public UpdateAttraction(WaterPosition position) { Position = position; }
    }

    public sealed class EndAttraction : IDeskFishCommand { }
}
