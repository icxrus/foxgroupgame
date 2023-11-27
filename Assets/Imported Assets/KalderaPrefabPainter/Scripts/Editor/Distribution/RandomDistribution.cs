using CollisionBear.WorldEditor.Lite.Generation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Distribution
{
    public class RandomDistribution : DistributionBase
    {
        public override string Name => "Even Random";

        protected override string ToolTip => "Evenly distributes items over an area, but with some randomness.";

        protected override string ButtonImagePath => "Icons/IconDistributionRandom.png";

        public RandomDistribution(int index) : base(index) { }

        public override List<Vector2> GetPoints(float size, float spacing, IGenerationBounds boundsProvider)
        {
            return new GeneratedPositions(size, spacing, boundsProvider).GetPoints();
        }

        public class GeneratedPositions
        {
            private float ItemSizeSquare;
            private float LocationSize;

            private List<BoxRect> GridBoxes;
            private List<Vector2> GridPositions;
            private IGenerationBounds BoundsProvider;

            private int MaxAllowedBoxFailCount;

            public int Count => GridPositions.Count;

            public GeneratedPositions(float locationSize, float itemSize, IGenerationBounds boundsProvider, int maxAllowedBoxFailCount = 2)
            {
                ItemSizeSquare = GetItemSize(itemSize);
                LocationSize = locationSize;
                BoundsProvider = boundsProvider;
                MaxAllowedBoxFailCount = maxAllowedBoxFailCount;

                var gridSize = GetGridSize(itemSize);
                GridBoxes = GetGridBoxesWithinBounds(LocationSize, gridSize);
                RandomizePoints();

                GridPositions = GridBoxes
                    .Where(b => b.PlacedPoint.HasValue)
                    .Select(p => p.PlacedPoint.Value)
                    .ToList();
            }

            public List<Vector2> GetPoints() => GridPositions;
            public Vector2 GetPoint(int index) => GridPositions[index];

            private float GetItemSize(float itemSize) => itemSize * itemSize;

            private float GetGridSize(float objectSize)
            {
                return objectSize / (Mathf.Sqrt(2));
            }

            private void RandomizePoints()
            {
                var remainingBoxes = new List<BoxRect>(GridBoxes);

                while (remainingBoxes.Count > 0) {
                    var randomBox = GetRandomBox(remainingBoxes);
                    var point = randomBox.GetRandomPointWithinBox();

                    if (OverlapExists(point, randomBox.GetPointsFromNeightbours(), ItemSizeSquare)) {
                        randomBox.FailCount++;
                        if (randomBox.FailCount > MaxAllowedBoxFailCount) {
                            remainingBoxes.Remove(randomBox);
                        }
                    } else {
                        remainingBoxes.Remove(randomBox);
                        randomBox.PlacedPoint = point;
                    }
                }

                GridBoxes = GridBoxes.OrderBy(b => Random.Range(0, 1000)).ToList();
            }

            private BoxRect GetRandomBox(List<BoxRect> boxes)
            {
                var index = Random.Range(0, boxes.Count);
                return boxes[index];
            }

            private bool OverlapExists(Vector2 point, List<Vector2> neightbourPoints, float objectSizeSqr)
            {
                foreach (var neighbourPoint in neightbourPoints) {
                    if ((point - neighbourPoint).sqrMagnitude < objectSizeSqr) {
                        return true;
                    }
                }

                return false;
            }

            private List<BoxRect> GetGridBoxesWithinBounds(float locationSize, float gridSize)
            {
                var boxesInSquare = GetGridBoxesForSquare(locationSize, gridSize);

                var result = new List<BoxRect>();
                foreach (var box in boxesInSquare) {
                    if (BoundsProvider.IsWithinBounds(locationSize, box)) {
                        result.Add(box);
                    } else {
                        box.IsDeleted = true;
                    }
                }

                foreach (var box in result) {
                    box.UpdateNeighbours();
                }

                return result;
            }

            private List<BoxRect> GetGridBoxesForSquare(float squareSize, float gridSize)
            {
                var boxesInRow = Mathf.FloorToInt(squareSize / gridSize);

                var result = new List<BoxRect>();
                for (var y = -boxesInRow; y < boxesInRow; y++) {
                    for (var x = -boxesInRow; x < boxesInRow; x++) {
                        result.Add(new BoxRect(x * gridSize, y * gridSize, gridSize, gridSize));
                    }
                }

                for (var i = 0; i < result.Count; i++) {
                    var box = result[i];
                    if (i - boxesInRow >= 0) {
                        box.Neighbours.Add(result[i - boxesInRow]);

                        if (i % boxesInRow != 0) {
                            box.Neighbours.Add(result[i - boxesInRow - 1]);
                        }

                        if (i % boxesInRow != boxesInRow - 1) {
                            box.Neighbours.Add(result[i - boxesInRow + 1]);
                        }
                    }

                    if (i + boxesInRow < result.Count) {
                        box.Neighbours.Add(result[i + boxesInRow]);

                        if (i % boxesInRow != 0) {
                            box.Neighbours.Add(result[i + boxesInRow - 1]);
                        }

                        if (i % boxesInRow != boxesInRow - 1) {
                            box.Neighbours.Add(result[i + boxesInRow + 1]);
                        }
                    }

                    if (i % boxesInRow != 0) {
                        box.Neighbours.Add(result[i - 1]);
                    }

                    if (i % boxesInRow != boxesInRow - 1) {
                        box.Neighbours.Add(result[i + 1]);
                    }
                }

                return result;
            }
        }
    }
}