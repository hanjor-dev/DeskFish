using UnityEngine;

namespace DeskFish.Runtime.Prototype
{
    public sealed class TankInteraction : MonoBehaviour
    {
        [SerializeField] private Camera tankCamera;
        [SerializeField] private TankPrototype tank;

        private void Update()
        {
            if (tankCamera == null) tankCamera = Camera.main;
            if (tank == null) tank = FindFirstObjectByType<TankPrototype>();
            if (!Input.GetMouseButtonDown(0) || tankCamera == null || tank == null) return;
            if (!tankCamera.isActiveAndEnabled || tankCamera.rect.width <= 0f || tankCamera.rect.height <= 0f || tankCamera.pixelWidth <= 0 || tankCamera.pixelHeight <= 0) return;
            var worldPosition = tankCamera.ScreenToWorldPoint(Input.mousePosition);
            tank.CreateRipple(worldPosition);
            foreach (var fish in FindObjectsByType<FishAgent>(FindObjectsSortMode.None)) fish.ReactToClick(worldPosition);
        }
    }
}
