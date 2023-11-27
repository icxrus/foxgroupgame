using CollisionBear.WorldEditor.Lite.Utils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Brushes
{
    public class EraserBrush : BrushBase
    {
        private static readonly List<PlacementInformation> EmptyPlacementList = new List<PlacementInformation>();

        private static GameObject[] GameObjectsCache = new GameObject[100];

        private static readonly Color PassiveHandleBrushColor = new Color(1f, 1f, 0f, 0.1f);
        private static readonly Color PassiveHandleOutlineColor = new Color(1f, 1f, 0f, 0.5f);

        private static readonly Color ActiveHandleBrushColor = new Color(1f, 0.2f, 0f, 0.1f);
        private static readonly Color ActiveHandleOutlineColor = new Color(1f, 0.2f, 0f, 0.5f);

        public const float SprayIntensityMin = 1f;
        public const float SprayIntensityMax = 100f;

        [System.Serializable]
        public class EraserBrushSettings
        {
            public float BrushSize = AreaBrushSettings.BrushSizePresets[1].BrushSize;
        }

        public override string Name => "Eraser brush";
        public override KeyCode HotKey => KeyCode.U;

        protected override string ToolTip => "Erases prefabs in the selected area";
        protected override string ButtonImagePath => "Icons/IconEraser.png";

        private bool IsActive;
        private bool IsDragging;

        private EraserBrushSettings Settings = new EraserBrushSettings();

        public EraserBrush(int index) : base(index)
        {
        }

        public override bool ShowBrush(ScenePlacer scenePlacer) => IsActive;

        public override void OnSelected(ScenePlacer placer)
        {
            IsActive = true;
        }

        public override void OnClearSelection(ScenePlacer placer)
        {
            IsActive = false;
        }

        public override void DrawBrushEditor(ScenePlacer placer)
        {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField(KalderaEditorUtils.BrushSizeContent, GUILayout.Width(KalderaEditorUtils.OptionLabelWidth));
                var tmpBrushSize = EditorGUILayout.Slider(Settings.BrushSize, AreaBrushBase.BrushSizeMin, AreaBrushBase.BrushSizeMax);
                if (tmpBrushSize != Settings.BrushSize) {
                    SetBrushSize(tmpBrushSize, placer);
                }
            }
        }

        public override void DrawAdditionalSettings(ScenePlacer placer, SelectionSettings settings) { }

        protected override void UpdatePlacementPoints(Vector3 position, SelectionSettings selectionSettings, PlacementCollection placementCollection) { }

        protected override List<Vector2> GetPlacementOffsetValues(Vector3 position, SelectionSettings selectionSettings) => EmptyPointList;

        protected override List<PlacementInformation> PlacementsToPlace(ScenePlacer placer) => EmptyPlacementList;

        public override void DrawBrushHandle(Vector3 placementPosition, Vector3 mousePosition)
        {
            if(IsDragging) {
                Handles.color = ActiveHandleBrushColor;
                Handles.DrawSolidDisc(placementPosition, Vector3.up, Settings.BrushSize);

                Handles.color = ActiveHandleOutlineColor;
                Handles.DrawWireDisc(placementPosition, Vector3.up, Settings.BrushSize);
            } else {
                Handles.color = PassiveHandleBrushColor;
                Handles.DrawSolidDisc(placementPosition, Vector3.up, Settings.BrushSize);

                Handles.color = PassiveHandleOutlineColor;
                Handles.DrawWireDisc(placementPosition, Vector3.up, Settings.BrushSize);
            }
        }

        public override void DrawSceneHandleText(Vector2 screenPosition, Vector3 worldPosition, ScenePlacer placer) {
        }

        public override bool HandleKeyEvents(Event currentEvent, ScenePlacer placer)
        {
            if (currentEvent.type == EventType.KeyDown && currentEvent.shift) {
                foreach (var preset in AreaBrushSettings.BrushSizePresets) {
                    if (currentEvent.keyCode == preset.Hotkey) {
                        SetBrushSize(preset.BrushSize, placer);
                        return true;
                    }
                }
            }

            return false;
        }

        public override void StartPlacement(Vector3 position, ScenePlacer placer)
        {
            DistanceUtility.SegmentMap();
            EraseObjectsInRange(position);
            IsDragging = true;
        }

        public override void ActiveDragPlacement(Vector3 worldPosition, SelectionSettings settings, double deltaTime, ScenePlacer placer)
        {
            UpdatePosition(worldPosition, placer);
            EraseObjectsInRange(worldPosition);
        }

        private void EraseObjectsInRange(Vector3 worldPosition)
        {
            var amount = DistanceUtility.GetGameObjectsInRangeNonAlloc(worldPosition, Settings.BrushSize, GameObjectsCache);

            for (int i = 0; i < amount; i++) {
                var gameObject = GameObjectsCache[i];
                if (gameObject == null) {
                    continue;
                }

                if (gameObject != PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject)) {
                    continue;
                }

                Undo.DestroyObjectImmediate(gameObject);
            }
        }

        public override void StaticDragPlacement(Vector3 position, SelectionSettings settings, double deltaTime, ScenePlacer placer)
        {
            UpdatePosition(position, placer);
        }

        private void UpdatePosition(Vector3 worldPosition, ScenePlacer placer) {
            placer.MovePosition(placer.ScreenPosition, worldPosition);
        }

        public override List<GameObject> EndPlacement(Vector3 position, GameObject parentCollider, SelectionSettings settings, ScenePlacer placer)
        {
            IsDragging = false;
            return EmptyGameObjectList;
        }

        private void SetBrushSize(float size, ScenePlacer placer)
        {
            Settings.BrushSize = size;
            placer.NotifyChange();
        }
    }
}