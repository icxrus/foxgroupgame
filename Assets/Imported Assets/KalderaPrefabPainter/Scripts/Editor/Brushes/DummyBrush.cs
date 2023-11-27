using System.Collections.Generic;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Brushes
{
    public class DummyBrush : BrushBase
    {
        private string BrushName;
        private KeyCode BrushHotKey;
        private string BrushToolTip;
        private string ImagePath;

        public override string Name => BrushName;

        public override KeyCode HotKey => BrushHotKey;

        protected override string ToolTip => BrushToolTip;

        protected override string ButtonImagePath => ImagePath;

        public override bool Disabled => true;

        public DummyBrush(int index, string brushName, KeyCode hotkey, string toolTip, string imagePath): base(index)
        {
            BrushName = brushName;
            BrushHotKey = hotkey;
            BrushToolTip = toolTip;
            ImagePath = imagePath;
        }

        public override void DrawBrushEditor(ScenePlacer placer) { }

        protected override List<Vector2> GetPlacementOffsetValues(Vector3 position, SelectionSettings selectionSettings) => EmptyPointList;
    }
}
