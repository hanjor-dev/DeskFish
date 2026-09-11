using System;
using System.Collections.Generic;

namespace DeskFish.Runtime.Domain
{
    public sealed class Tank
    {
        private readonly List<Fish> fish = new List<Fish>();
        public IReadOnlyList<Fish> Fish => fish;

        public void AddFish(Fish value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            fish.Add(value);
        }

        public void Advance(TimeSpan elapsed, TimeSpan adultAge)
        {
            for (var i = 0; i < fish.Count; i++) fish[i].Advance(elapsed, adultAge);
        }

        public void SetAttraction(bool attracted)
        {
            for (var i = 0; i < fish.Count; i++) fish[i].SetAttracted(attracted);
        }
    }
}
