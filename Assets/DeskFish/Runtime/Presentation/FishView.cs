using UnityEngine;
using DeskFish.Runtime.Domain;

namespace DeskFish.Runtime.Presentation
{
    public sealed class FishView : MonoBehaviour
    {
        public string FishId { get; private set; }

        public void Bind(Fish fish)
        {
            FishId = fish.Id;
            gameObject.name = "FishView_" + fish.Species.Id;
        }
    }
}
