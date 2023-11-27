using CollisionBear.WorldEditor.Lite.Generation;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Brushes
{
    public class CircleBrush : AreaBrushBase
    {
        public class CircleGenerationBounds : IGenerationBounds
        {
            public bool IsWithinBounds(float circleSize, BoxRect box)
            {
                var circleSizeSquare = Mathf.Pow(circleSize, 2);
                return (box.TopLeft.sqrMagnitude < circleSizeSquare && box.BottomLeft.sqrMagnitude < circleSizeSquare && box.TopRight.sqrMagnitude < circleSizeSquare && box.BottomRight.sqrMagnitude < circleSizeSquare);
            }
        }

        public override string Name => "Circle brush";
        public override KeyCode HotKey => KeyCode.R;

        protected override string ToolTip => "Places multiple objects (always at least 1) in a circle";
        protected override string ButtonImagePath => "Icons/IconGridCircle.png";

        public CircleBrush(int index) : base(index)
        {
            GenerationBounds = new CircleGenerationBounds();
        }

        public override void DrawBrushHandle(Vector3 placementPosition, Vector3 mousePosition)
        {
            Handles.color = HandleBrushColor;
            Handles.DrawSolidDisc(placementPosition, Vector3.up, Settings.BrushSize);
            Handles.color = HandleOutlineColor;
            Handles.DrawWireDisc(placementPosition, Vector3.up, Settings.BrushSize);

            if (HasDrag(StartDragPosition, EndDragPosition)) {
                DrawRotationCompass(StartDragPosition.Value, EndDragPosition.Value);
                DrawRotationArrow(StartDragPosition.Value, EndDragPosition.Value);
            }
        }
    }
}
