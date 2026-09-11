using System.Collections.Generic;
using UnityEngine;

namespace DeskFish.Runtime.Prototype
{
    public sealed class TankPrototype : MonoBehaviour
    {
        [SerializeField] private int fishCount = 3;
        [SerializeField] private Vector2 tankSize = new Vector2(16f, 9f);
        [SerializeField] private Color waterColor = new Color(0.06f, 0.27f, 0.36f, 0.92f);
        private readonly List<FishAgent> fish = new List<FishAgent>();
        private Sprite whiteSprite;

        private void Awake()
        {
            whiteSprite = CreateSprite("PrototypePixel", Color.white);
            EnsureCamera();
            BuildTank();
        }

        private static void EnsureCamera()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
            }

            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.transform.rotation = Quaternion.identity;
            camera.rect = new Rect(0f, 0f, 1f, 1f);
            camera.targetDisplay = 0;
            camera.cullingMask = ~0;
            camera.enabled = true;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(1f, 0f, 1f, 1f);
        }

        private void BuildTank()
        {
            CreateRect("Water", new Vector2(0f, 0.25f), tankSize - new Vector2(0.5f, 1.2f), waterColor, -5);
            CreateRect("Sand", new Vector2(0f, -3.55f), new Vector2(15.5f, 1.1f), new Color(0.72f, 0.57f, 0.36f), -2);
            CreateRect("GlassFrame", new Vector2(0f, -0.1f), new Vector2(16.4f, 8.9f), new Color(0.35f, 0.75f, 0.82f, 0.08f), 5);

            for (var i = 0; i < 7; i++)
            {
                var plant = CreateRect("WaterPlant", new Vector2(-7f + i * 2.2f, -2.7f + (i % 2) * 0.2f), new Vector2(0.22f, 2.2f + (i % 3) * 0.5f), new Color(0.1f, 0.52f, 0.29f, 0.9f), -1);
                plant.transform.rotation = Quaternion.Euler(0f, 0f, (i % 2 == 0 ? -1f : 1f) * 7f);
            }

            for (var i = 0; i < 10; i++)
            {
                var bubble = CreateRect("Bubble", new Vector2(-7.2f + (i % 5) * 3.3f, -2.4f + (i % 3) * 0.25f), Vector2.one * 0.12f, new Color(0.65f, 0.95f, 1f, 0.72f), 1);
                bubble.AddComponent<BubbleMotion>().Initialize(-2.5f + (i % 3), 2f + (i % 4) * 0.3f);
            }

            for (var i = 0; i < fishCount; i++)
            {
                var fishObject = new GameObject($"Fish_{i + 1}");
                fishObject.transform.SetParent(transform);
                fishObject.transform.position = new Vector3(-4.8f + i * 3.8f, 0.7f + (i % 2) * 1.2f, -3f);
                var renderer = fishObject.AddComponent<SpriteRenderer>();
                renderer.sprite = CreateSprite($"FishSprite_{i}", i % 2 == 0 ? new Color(0.95f, 0.62f, 0.22f) : new Color(0.32f, 0.76f, 0.9f));
                renderer.sortingOrder = 10 + i;
                var agent = fishObject.AddComponent<FishAgent>();
                agent.Initialize(new Vector2(-7.4f, -2.8f), new Vector2(7.4f, 3f), 1.0f + i * 0.12f, i % 2 == 0);
                fish.Add(agent);
            }
        }

        public void CreateRipple(Vector2 worldPosition)
        {
            var ripple = CreateRect("Ripple", worldPosition, new Vector2(0.15f, 0.15f), new Color(0.7f, 0.95f, 1f, 0.8f), 20);
            ripple.AddComponent<RippleMotion>();
        }

        private GameObject CreateRect(string objectName, Vector2 position, Vector2 size, Color color, int sortingOrder)
        {
            var objectInstance = new GameObject(objectName);
            objectInstance.transform.SetParent(transform);
            objectInstance.transform.localPosition = new Vector3(position.x, position.y, 0f);
            objectInstance.transform.localScale = size;
            var renderer = objectInstance.AddComponent<SpriteRenderer>();
            renderer.sprite = whiteSprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return objectInstance;
        }

        private static Sprite CreateSprite(string spriteName, Color color)
        {
            var texture = new Texture2D(64, 64, TextureFormat.RGBA32, false) { name = spriteName };
            var pixels = new Color[64 * 64];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = color;
            texture.SetPixels(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64f);
        }
    }

    public sealed class BubbleMotion : MonoBehaviour
    {
        private float top;
        private float speed;
        public void Initialize(float topHeight, float riseSpeed) { top = topHeight; speed = riseSpeed; }
        private void Update()
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            if (transform.localPosition.y > top) transform.localPosition = new Vector3(transform.localPosition.x, -2.8f, 0f);
        }
    }

    public sealed class RippleMotion : MonoBehaviour
    {
        private float life = 0.65f;
        private void Update()
        {
            life -= Time.deltaTime;
            transform.localScale += Vector3.one * Time.deltaTime * 2.2f;
            var renderer = GetComponent<SpriteRenderer>();
            renderer.color = new Color(0.7f, 0.95f, 1f, Mathf.Clamp01(life));
            if (life <= 0f) Destroy(gameObject);
        }
    }
}
