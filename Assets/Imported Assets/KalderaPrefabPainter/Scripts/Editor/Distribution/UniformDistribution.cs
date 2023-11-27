using CollisionBear.WorldEditor.Lite.Generation;
using System.Collections.Generic;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Distribution
{
    public class UniformDistribution : DistributionBase
    {
        public override string Name => "Grid";

        protected override string ToolTip => "Distributes item exactly evenly, filling the area in a grid pattern";

        protected override string ButtonImagePath => "Icons/IconDistributionUniform.png";

        public UniformDistribution(int index) : base(index) { }

        public override List<Vector2> GetPoints(float size, float spacing, IGenerationBounds boundsProvider)
        {
            var itemCount = Mathf.FloorToInt(size / spacing);

            var result = new List<Vector2>();

            for (var y = -itemCount; y < itemCount; y++) {
                for (var x = -itemCount; x < itemCount; x++) {
                    var position = new Vector2(x * spacing, y * spacing);
                    if (boundsProvider.IsWithinBounds(size, new BoxRect(position, Vector2.zero))) {
                        result.Add(new Vector2(x * spacing, y * spacing));
                    }
                }
            }

            return result;
        }
    }
}