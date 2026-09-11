using System;

namespace DeskFish.Runtime.Domain
{
    public enum FishGrowthStage
    {
        Juvenile,
        Adult
    }

    public sealed class Fish
    {
        public string Id { get; }
        public FishSpecies Species { get; }
        public string Name { get; private set; }
        public FishGrowthStage GrowthStage { get; private set; }
        public DateTime BornAtUtc { get; }
        public TimeSpan LivedFor { get; private set; }
        public int FeedingCount { get; private set; }
        public bool IsAttracted { get; private set; }

        public Fish(string id, FishSpecies species, string name, DateTime bornAtUtc)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("Fish id is required.", nameof(id));
            if (species == null) throw new ArgumentNullException(nameof(species));
            Id = id;
            Species = species;
            Name = name ?? species.DisplayName;
            BornAtUtc = bornAtUtc.ToUniversalTime();
            GrowthStage = FishGrowthStage.Juvenile;
        }

        public void Advance(TimeSpan elapsed, TimeSpan adultAge)
        {
            if (elapsed <= TimeSpan.Zero) return;
            LivedFor += elapsed;
            if (LivedFor >= adultAge) GrowthStage = FishGrowthStage.Adult;
        }

        public void Feed()
        {
            FeedingCount++;
        }

        public void SetAttracted(bool attracted)
        {
            IsAttracted = attracted;
        }
    }
}
