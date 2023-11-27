using System.Collections.Generic;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Utils
{
    public static class DistanceUtility
    {
        private const float BoxSize = 10f;
        private const int BoxCount = 1000;
        private const int BoxOffset = BoxCount / 2;

        private class DistanceBox
        {
            public int X;
            public int Y;

            public List<GameObject> GameObjects = new List<GameObject>();

            public DistanceBox(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        private static DistanceBox[,] Boxes;

        private static List<DistanceBox> GetBoxes(Vector3 position, float radius)
        {
            var result = new List<DistanceBox>();
            var range = Mathf.CeilToInt(radius / BoxSize);
            var xOffset = GetBoxIndex(position.x);
            var yOffset = GetBoxIndex(position.z);

            for (int y = yOffset - range; y <= yOffset + range; y++) {
                for (int x = xOffset - range; x <= xOffset + range; x++) {
                    if(x < 0 || x >= BoxCount || y < 0 || y >= BoxCount) {
                        continue;
                    }

                    if (Boxes[x, y] != null) {
                        result.Add(Boxes[x, y]);
                    }
                }
            }

            return result;
        }


        private static DistanceBox GetBox(Vector3 position)
        {
            var x = GetBoxIndex(position.x);
            var y = GetBoxIndex(position.z);

            if (Boxes[x, y] == null) {
                Boxes[x, y] = new DistanceBox(x, y);
            }

            return Boxes[x, y];
        }

        private static int GetBoxIndex(float position) => Mathf.RoundToInt(position / BoxSize) + BoxOffset;

        public static void SegmentMap()
        {
            Boxes = new DistanceBox[BoxCount, BoxCount];
            var gameObjects = GameObject.FindObjectsOfType<GameObject>();

            foreach (var gameObject in gameObjects) {
   
                GetBox(gameObject.transform.position).GameObjects.Add(gameObject);
            }
        }

        public static void RemoveObjects(GameObject[] gameObjectsCache, int index)
        {
            for(int i = 0; i < index; i ++) {
                var gameObject = gameObjectsCache[i];
                var box = GetBox(gameObject.transform.position);
                box.GameObjects.Remove(gameObject);
            }
        }

        public static int GetGameObjectsInRangeNonAlloc(Vector3 position, float radius, GameObject[] result)
        {
            var boxes = GetBoxes(position, radius);
            var radiusSqr = radius * radius;

            var index = 0;
            foreach(var box in boxes) { 
                foreach (var gameObject in box.GameObjects) {
                    if (gameObject == null) {
                        continue;
                    }

                    if (index >= result.Length) {
                        break;
                    }

                    var deltaPosition = gameObject.transform.position - position;
                    deltaPosition.y = 0;
                    if (deltaPosition.sqrMagnitude < radiusSqr) {
                        result[index] = gameObject;
                        index++;
                    }
                }
            }

            return index;
        }
    }
}