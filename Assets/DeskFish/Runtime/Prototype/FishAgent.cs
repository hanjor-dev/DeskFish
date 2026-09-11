using UnityEngine;

namespace DeskFish.Runtime.Prototype
{
    public sealed class FishAgent : MonoBehaviour
    {
        private Vector2 minimum;
        private Vector2 maximum;
        private float speed;
        private bool curious;
        private Vector2 target;
        private float retargetTimer;

        public void Initialize(Vector2 minBounds, Vector2 maxBounds, float movementSpeed, bool isCurious)
        {
            minimum = minBounds;
            maximum = maxBounds;
            speed = movementSpeed;
            curious = isCurious;
            PickTarget();
        }

        private void Update()
        {
            retargetTimer -= Time.deltaTime;
            if (retargetTimer <= 0f) PickTarget();

            var direction = target - (Vector2)transform.localPosition;
            if (direction.sqrMagnitude < 0.08f) PickTarget();
            else
            {
                var movement = direction.normalized * speed * Time.deltaTime;
                transform.localPosition += (Vector3)movement;
                if (Mathf.Abs(direction.x) > 0.05f) transform.localScale = new Vector3(Mathf.Sign(direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
            }
        }

        private void PickTarget()
        {
            retargetTimer = Random.Range(1.2f, 3.5f);
            var verticalBias = curious ? Random.Range(-0.5f, 1.8f) : Random.Range(-1.7f, 1.1f);
            target = new Vector2(Random.Range(minimum.x, maximum.x), Mathf.Clamp(verticalBias, minimum.y, maximum.y));
        }

        public void ReactToClick(Vector2 clickPosition)
        {
            var away = (Vector2)transform.position - clickPosition;
            target = Vector2.ClampMagnitude((Vector2)transform.position + away.normalized * 2.2f, 10f);
            retargetTimer = 0.35f;
        }
    }
}
