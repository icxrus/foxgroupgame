using System;
using System.Linq;
using UnityEngine;


namespace CollisionBear.WorldEditor.Lite.Extensions
{
    public static class ObjectExtension
    {
        public static bool In<T>(this T item, params T[] items)
        {
            if (items == null) {
                throw new ArgumentNullException();
            }

            return items.Contains(item);
        }

        private static RaycastHit[] RaycasyHits = new RaycastHit[256];

        public static int CastNonAlloc(this Collider collider, Collider[] collidersOut)
        {
            if (collider is BoxCollider) {
                return ((BoxCollider)collider).CastNonAlloc(collidersOut);
            } else if (collider is SphereCollider) {
                return ((SphereCollider)collider).CastNonAlloc(collidersOut);
            } else {
                throw new NotSupportedException("Collider.CastNonAlloc can only be used with BoxColliders and SphereColliders");
            }
        }

        public static int CastNonAlloc(this BoxCollider collider, Collider[] collidersOut)
        {
            var center = collider.center + collider.gameObject.transform.position;
            var halfExtents = (collider.size) / 2;

            var hitCount = Physics.BoxCastNonAlloc(center, halfExtents, Vector3.up, RaycasyHits);
            var resultHitCount = 0;

            for (int i = 0; i < hitCount; i++) {
                collidersOut[resultHitCount] = RaycasyHits[i].collider;
                resultHitCount++;
            }

            return resultHitCount;
        }

        public static int CastNonAlloc(this SphereCollider collider, Collider[] collidersOut)
        {
            var center = collider.center + collider.gameObject.transform.position;
            var radius = (collider.radius) / 2;

            var hitCount = Physics.SphereCastNonAlloc(center, radius, Vector3.up, RaycasyHits);
            var resultHitCount = 0;

            for (int i = 0; i < hitCount; i++) {
                collidersOut[resultHitCount] = RaycasyHits[i].collider;
                resultHitCount++;
            }

            return resultHitCount;
        }

        public static float GetRendererBoundsSize(this Renderer renderer)
        {
            var size = renderer.bounds.size;
            var result = new Vector2(size.y, size.z).magnitude;
            return result;
        }
    }
}