using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Generation
{
    public class BoxRect
    {
        private float X;
        private float Y;
        private float Width;
        private float Height;

        public Vector2 TopLeft;
        public Vector2 BottomLeft;
        public Vector2 TopRight;
        public Vector2 BottomRight;
        public Vector2 Center;

        public int FailCount = 0;

        public Vector2? PlacedPoint = null;
        public List<BoxRect> Neighbours = new List<BoxRect>();

        public bool IsDeleted = false;

        public BoxRect(Vector2 position, Vector2 size): this(position.x, position.y, size.x, size.y) { }

        public BoxRect(float x, float y, float width, float height)
        {
            Center = new Vector2(x, y) + new Vector2(Width, Height) * 0.5f;
            X = x;
            Y = y;
            Width = width;
            Height = height;

            TopLeft = new Vector2(x, y);
            BottomLeft = new Vector2(x, y + height);
            TopRight = new Vector2(x + width, y);
            BottomRight = new Vector2(x + width, y + height);
        }

        public Vector2 GetRandomPointWithinBox()
        {
            return new Vector2(X + Random.Range(0, Width), Y + Random.Range(0, Height));
        }

        public List<Vector2> GetPointsFromNeightbours()
        {
            var result = new List<Vector2>();
            foreach (var neighbours in Neighbours) {
                if (neighbours.PlacedPoint.HasValue) {
                    result.Add(neighbours.PlacedPoint.Value);
                }
            }

            return result;
        }

        public void UpdateNeighbours()
        {
            Neighbours = Neighbours.Where(n => !n.IsDeleted).ToList();
        }
    }
}
