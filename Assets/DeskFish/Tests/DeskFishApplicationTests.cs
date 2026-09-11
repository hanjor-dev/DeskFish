using System;
using NUnit.Framework;
using DeskFish.Runtime.Application;
using DeskFish.Runtime.Domain;

namespace DeskFish.Tests
{
    public sealed class DeskFishApplicationTests
    {
        private static readonly DateTime Start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [Test]
        public void NewSessionStartsWithThreeDistinctFish()
        {
            var application = DeskFishApplication.StartNew(Start);

            Assert.That(application.Tank.Fish.Count, Is.EqualTo(3));
            Assert.That(application.Tank.Fish[0].Species.Id, Is.Not.EqualTo(application.Tank.Fish[1].Species.Id));
            Assert.That(application.Tank.Fish[1].Species.Id, Is.Not.EqualTo(application.Tank.Fish[2].Species.Id));
        }

        [Test]
        public void ClockDoesNotRewardTimeGoingBackwardsAndClampsOfflineAdvance()
        {
            var application = DeskFishApplication.StartNew(Start);

            Assert.That(application.AdvanceTo(Start.AddHours(-1)), Is.EqualTo(TimeSpan.Zero));
            Assert.That(application.AdvanceTo(Start.AddDays(2)), Is.EqualTo(TimeSpan.FromHours(24)));
        }

        [Test]
        public void AttractionIsPublishedAndChangesFishState()
        {
            var application = DeskFishApplication.StartNew(Start);
            IDeskFishEvent received = null;
            application.EventPublished += value => received = value;

            application.Execute(new StartAttraction(new WaterPosition(1f, 2f)));

            Assert.That(received, Is.TypeOf<AttractionChanged>());
            Assert.That(application.Tank.Fish[0].IsAttracted, Is.True);
        }
    }
}
