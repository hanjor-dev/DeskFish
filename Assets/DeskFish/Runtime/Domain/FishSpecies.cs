namespace DeskFish.Runtime.Domain
{
    public sealed class FishSpecies
    {
        public string Id { get; }
        public string DisplayName { get; }
        public string PresentationKey { get; }

        public FishSpecies(string id, string displayName, string presentationKey)
        {
            Id = id;
            DisplayName = displayName;
            PresentationKey = presentationKey;
        }
    }
}
