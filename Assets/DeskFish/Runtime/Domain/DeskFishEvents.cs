namespace DeskFish.Runtime.Domain
{
    public interface IDeskFishEvent { }

    public sealed class SessionStarted : IDeskFishEvent
    {
        public Tank Tank { get; }
        public SessionStarted(Tank tank) { Tank = tank; }
    }

    public sealed class WaterClicked : IDeskFishEvent
    {
        public WaterPosition Position { get; }
        public WaterClicked(WaterPosition position) { Position = position; }
    }

    public sealed class AttractionChanged : IDeskFishEvent
    {
        public WaterPosition Position { get; }
        public bool IsActive { get; }
        public AttractionChanged(WaterPosition position, bool isActive)
        {
            Position = position;
            IsActive = isActive;
        }
    }

    public struct WaterPosition
    {
        public float X { get; }
        public float Y { get; }
        public WaterPosition(float x, float y) { X = x; Y = y; }
    }
}
