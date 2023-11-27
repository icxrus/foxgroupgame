using CollisionBear.WorldEditor.Lite.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Brushes
{
    public class SingleBrush : BrushBase
    {
        public class SingleBrushSettings
        {
            public bool MaintainRotation = false;
        }

        public override string Name => "Paint brush";
        public override KeyCode HotKey => KeyCode.Q;
        protected override string ToolTip => "Always place a single object";
        protected override string ButtonImagePath => "Icons/IconGridPoint.png";

        private SingleBrushSettings Settings = new SingleBrushSettings();

        public SingleBrush(int index) : base(index) { }

        public override void DrawBrushEditor(ScenePlacer placer)
        {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorCustomGUILayout.SetGuiBackgroundColorState(Settings.MaintainRotation);
                if (GUILayout.Button(KalderaEditorUtils.MaintainRotationContent, GUILayout.Width(KalderaEditorUtils.IconButtonSize), GUILayout.Height(KalderaEditorUtils.IconButtonSize))) {
                    Settings.MaintainRotation = !Settings.MaintainRotation;
                    placer.NotifyChange();
                }
            }
        }

        public override void DrawBrushHandle(Vector3 placementPosition, Vector3 adjustedWorldPosition)
        {
            if (HasDrag(StartDragPosition, EndDragPosition)) {
                DrawRotationCompass(StartDragPosition.Value, adjustedWorldPosition);
            }
        }

        public override void DrawSceneHandleText(Vector2 screenPosition, Vector3 worldPosition, ScenePlacer placer)
        {
            DrawHandleTextAtOffset(screenPosition, 0, new GUIContent($"Scale:\t {placer.CurrentBrush.ScaleFactor.ToString(FloatFormat)}"));
            DrawHandleTextAtOffset(screenPosition, 1, new GUIContent($"Rotation:\t {placer.CurrentBrush.Rotation.ToString(RotationFormat)}"));
            DrawHandleTextAtOffset(screenPosition, 2, KalderaEditorUtils.MousePlaceToolTip);
        }

        public override void ActiveDragPlacement(Vector3 position, SelectionSettings settings, double deltaTime, ScenePlacer placer)
        {
            EndDragPosition = position;
            Rotation = placer.RotateTowardsPosition(position);
        }

        public override void ShiftDragPlacement(Vector3 position, SelectionSettings settings, double deltaTime, ScenePlacer placer)
        {
            var snappedPosition = GetRotationSnappedPosition(GetDragPosition(placer.PlacementPosition, position), position);
            ActiveDragPlacement(snappedPosition, settings, deltaTime, placer);
        }

        public override List<GameObject> EndPlacement(Vector3 position, GameObject parentCollider, SelectionSettings settings, ScenePlacer placer)
        {
            LastRotation = placer.PlacementCollection.Placements.First().Rotation;
            var result = base.EndPlacement(GetDragPosition(StartDragPosition, position), parentCollider, settings, placer);
            StartDragPosition = null;
            EndDragPosition = null;
            return result;
        }

        private Vector3 GetDragPosition(Vector3? startDragPosition, Vector3 position)
        {
            if (startDragPosition.HasValue) {
                return startDragPosition.Value;
            } else {
                return position;
            }
        }

        public override void CycleVariant(int steps, ScenePlacer placer)
        {
            foreach (var placement in placer.PlacementCollection.Placements) {
                var validObjects = placement.Item.ValidObjects();
                var selectedVariantIndex = validObjects.IndexOf(placement.PrefabObject);

                if (selectedVariantIndex == -1) {
                    continue;
                }
                var nextIndex = (int)Mathf.Repeat(selectedVariantIndex + steps, validObjects.Count);
                placement.ReplacePlacementObject(nextIndex, BrushPosition, ScaleFactor);
            }
        }

        protected override List<Vector2> GetPlacementOffsetValues(Vector3 position, SelectionSettings _)
        {
            return new List<Vector2> { Vector2.zero };
        }

        protected override Vector3 GetItemRotation(Vector3 position, PaletteItem item, GameObject prefabObject)
        {
            var result = base.GetItemRotation(position, item, prefabObject);

            if (Settings.MaintainRotation && LastRotation.HasValue) {
                result = LastRotation.Value.eulerAngles;
            }

            return result;
        }
    }
}