using System.Collections.Generic;
using UnityEngine;
using DeskFish.Runtime.Application;
using DeskFish.Runtime.Domain;

namespace DeskFish.Runtime.Presentation
{
    public sealed class DeskFishPresentationBootstrap : MonoBehaviour
    {
        [SerializeField] private Vector2 fishBounds = new Vector2(7.2f, 3f);
        [SerializeField] private bool createCameraIfMissing = true;
        private DeskFishApplication application;
        private readonly List<FishView> views = new List<FishView>();

        private void Start()
        {
            application = DeskFishApplication.StartNew(System.DateTime.UtcNow);
            application.EventPublished += OnEventPublished;
            EnsureCamera();
            BuildDefaultFish();
        }

        private void Update()
        {
            if (application != null) application.AdvanceTo(System.DateTime.UtcNow);
        }

        private void BuildDefaultFish()
        {
            var colors = new[] { new Color(0.95f, 0.62f, 0.22f), new Color(0.25f, 0.72f, 0.95f), new Color(0.95f, 0.35f, 0.2f) };
            for (var i = 0; i < application.Tank.Fish.Count; i++)
            {
                var fishObject = new GameObject("FishView_" + (i + 1));
                fishObject.transform.SetParent(transform, false);
                fishObject.transform.localPosition = new Vector3(-4.5f + i * 4.5f, 0.5f + (i % 2) * 1.1f, -2f);
                fishObject.transform.localScale = new Vector3(1.25f + i * 0.1f, 0.7f, 1f);
                var renderer = fishObject.AddComponent<SpriteRenderer>();
                renderer.sprite = CreateSprite("DeskFish_Default_" + i);
                renderer.color = colors[i];
                renderer.sortingOrder = 10 + i;
                var view = fishObject.AddComponent<FishView>();
                view.Bind(application.Tank.Fish[i]);
                views.Add(view);
            }
        }

        private void OnEventPublished(IDeskFishEvent value)
        {
            var attraction = value as AttractionChanged;
            if (attraction == null) return;
            for (var i = 0; i < views.Count; i++)
            {
                var offset = (i - (views.Count - 1) * 0.5f) * 0.9f;
                var target = attraction.IsActive
                    ? new Vector2(attraction.Position.X + offset, attraction.Position.Y + (i % 2) * 0.25f)
                    : new Vector2(-4.5f + i * 4.5f, 0.5f + (i % 2) * 1.1f);
                views[i].transform.localPosition = new Vector3(target.x, target.y, views[i].transform.localPosition.z);
            }
        }

        private void EnsureCamera()
        {
            if (!createCameraIfMissing || Camera.main != null) return;
            var cameraObject = new GameObject("DeskFish Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
        }

        private static Sprite CreateSprite(string spriteName)
        {
            var texture = new Texture2D(32, 16, TextureFormat.RGBA32, false) { name = spriteName };
            var pixels = new Color[32 * 16];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            texture.SetPixels(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, 32f, 16f), new Vector2(0.5f, 0.5f), 16f);
        }
    }
}
