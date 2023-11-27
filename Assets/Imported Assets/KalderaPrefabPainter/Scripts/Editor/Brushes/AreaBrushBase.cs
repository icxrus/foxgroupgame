using CollisionBear.WorldEditor.Lite.Generation;
using CollisionBear.WorldEditor.Lite.Utils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Brushes
{
    public class AreaBrushSizePreset
    {
        public KeyCode Hotkey;
        public float BrushSize;
    }

    [System.Serializable]
    public class AreaBrushSettings
    {
        public static readonly IReadOnlyList<AreaBrushSizePreset> BrushSizePresets = new List<AreaBrushSizePreset> {
            new AreaBrushSizePreset { Hotkey = KeyCode.Alpha1, BrushSize = 1.0f },
            new AreaBrushSizePreset { Hotkey = KeyCode.Alpha2, BrushSize = 5.0f },
            new AreaBrushSizePreset { Hotkey = KeyCode.Alpha3, BrushSize = 10.0f },
            new AreaBrushSizePreset { Hotkey = KeyCode.Alpha4, BrushSize = 15.0f },
            new AreaBrushSizePreset { Hotkey = KeyCode.Alpha5, BrushSize = 20.0f }
        };

        public int DistributionTypeIndex = 0;
        public float BrushSize = BrushSizePresets[1].BrushSize;
        public float ObjectDensity = 1.0f;
    }

    public abstract class AreaBrushBase : BrushBase
    {
        public const float BrushSizeMin = 0.1f;
        public const float BrushSizeMax = 25;
        public const float BrushSpacingMin = 0.2f;
        public const float BrushSpacingMax = 10.0f;

        protected AreaBrushSettings Settings = new AreaBrushSettings();
        protected IGenerationBounds GenerationBounds;

        public AreaBrushBase(int index) : base(index) { }

        public override void StartPlacement(Vector3 position, ScenePlacer placer)
        {
            StartDragPosition = position;
            EndDragPosition = null;
            base.StartPlacement(position, placer);
        }

        public override void ActiveDragPlacement(Vector3 position, SelectionSettings settings, double deltaTime, ScenePlacer placer)
        {
            EndDragPosition = position;
            Rotation = placer.RotatatePlacement(position);
        }

        public override void ShiftDragPlacement(Vector3 position, SelectionSettings settings, double deltaTime, ScenePlacer placer)
        {
            if(!StartDragPosition.HasValue) {
                return;
            }

            var snappedPosition = GetRotationSnappedPosition(StartDragPosition.Value, position);
            ActiveDragPlacement(snappedPosition, settings, deltaTime, placer);
        }

        public override List<GameObject> EndPlacement(Vector3 position, GameObject parentCollider, SelectionSettings settings, ScenePlacer placer)
        {
            StartDragPosition = null;
            return base.EndPlacement(position, parentCollider, settings, placer);
        }

        public override void DrawBrushEditor(ScenePlacer placer)
        {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField(KalderaEditorUtils.BrushSizeContent, GUILayout.Width(KalderaEditorUtils.OptionLabelWidth));
                var tmpBrushSize = EditorGUILayout.Slider(Settings.BrushSize, BrushSizeMin, BrushSizeMax);
                if (tmpBrushSize != Settings.BrushSize) {
                    SetBrushSize(tmpBrushSize, placer);
                }
            }

            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField(KalderaEditorUtils.ObjectDensityContent, GUILayout.Width(KalderaEditorUtils.OptionLabelWidth));
                var tmpBrushSpacing = EditorGUILayout.Slider(Settings.ObjectDensity, BrushSpacingMin, BrushSpacingMax);
                if (tmpBrushSpacing != Settings.ObjectDensity) {
                    Settings.ObjectDensity = tmpBrushSpacing;
                    placer.NotifyChange();
                }
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField(KalderaEditorUtils.BrushDistributionContent, EditorStyles.boldLabel, GUILayout.Width(KalderaEditorUtils.OptionLabelWidth));
                EditorGUILayout.LabelField(ScenePlacer.GetDistributionModes()[placer.SelectionSettings.SelectedDistributionIndex].Name);
            }

            using (new EditorGUILayout.HorizontalScope()) {
                foreach(var distributionMode in ScenePlacer.GetDistributionModes()) {
                    EditorCustomGUILayout.SetGuiBackgroundColorState(placer.CurrentDistribution == distributionMode);
                    if (GUILayout.Button(distributionMode.GetGUIContent(), GUILayout.Width(KalderaEditorUtils.IconButtonSize), GUILayout.Height(KalderaEditorUtils.IconButtonSize))) {
                        placer.SelectionSettings.SelectedDistributionIndex = distributionMode.Index;
                        placer.NotifyChange();
                    }
                }
            }

            EditorCustomGUILayout.RestoreGuiColor();
        }

        public override void DrawSceneHandleText(Vector2 screenPosition, Vector3 worldPosition, ScenePlacer placer)
        {
            DrawHandleTextAtOffset(screenPosition, 0, new GUIContent($"Object count: {placer.PlacementCollection.Placements.Count}"));
            DrawHandleTextAtOffset(screenPosition, 1, KalderaEditorUtils.MousePlaceToolTip);
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

        protected override List<Vector2> GetPlacementOffsetValues(Vector3 position, SelectionSettings selectionSettings)
        {
            var spacing = selectionSettings.GetSelectedItemSize() * (1f / Settings.ObjectDensity);
            return ScenePlacer
                .GetDistributionModes()[selectionSettings.SelectedDistributionIndex]
                .GetPoints(Settings.BrushSize, spacing, GenerationBounds);
        }

        private void SetBrushSize(float size, ScenePlacer placer)
        {
            Settings.BrushSize = size;
            placer.NotifyChange();
        }
    }
}
