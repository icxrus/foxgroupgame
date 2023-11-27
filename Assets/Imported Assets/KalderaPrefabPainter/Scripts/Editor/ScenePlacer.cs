using CollisionBear.WorldEditor.Lite.Brushes;
using CollisionBear.WorldEditor.Lite.Distribution;
using CollisionBear.WorldEditor.Lite.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite
{
    [System.Serializable]
    public class ScenePlacer
    {
        private static List<BrushBase> Brushes;
        private static List<IPlacementMode> PlacementModes;
        private static List<DistributionBase> Distributions;
        private static GUIContent[] PlacementModeGUIContent;

        private List<PaletteItem> PreviousSelection;

        public static IReadOnlyList<BrushBase> GetBrushMapping() { 
            if(Brushes == null) {
                Brushes = new List<BrushBase> {
                    new SingleBrush(index: 0),
                    new DummyBrush(index: 1, "Line brush", KeyCode.None, "[Only in full version]\nPlaces multiple objects in a line from where you start drag to the end", "Icons/IconGridLine.png"),
                    new DummyBrush(index: 2, "Path brush", KeyCode.None, "[Only in full version]\nPlaces multiple objects in a path along the mouse drag", "Icons/IconPathTool.png"),
                    new CircleBrush(index: 3),
                    new DummyBrush(index: 4, "Square brush", KeyCode.None, "[Only in full version]\nPlaces multiple objects (always at least 1) in a square", "Icons/IconGridSquare.png"),
                    new DummyBrush(index: 5, "Spray brush", KeyCode.None, "[Only in full version]\nSlowly plots down objects while keeping the mouse button pressed", "Icons/IconGridSpray.png"),
                    new EraserBrush(index: 6)
                };
            }
                
            return Brushes;
        }

        public static IReadOnlyList<IPlacementMode> GetPlacementModes() {
            if(PlacementModes == null) {
                PlacementModes = new List<IPlacementMode> {
                    new AnyColliderMode(),
                    new ParentObjectPlacementMode(),
                    new LayerObjectPlacementMode()
                };
            }

            return PlacementModes;
        }

        public static IReadOnlyList<DistributionBase> GetDistributionModes()  { 
            if(Distributions == null) {
                Distributions = new List<DistributionBase> {
                    new RandomDistribution(0),
                    new PerlinNoiseDistribution(1),
                    new UniformDistribution(2),
                };
            }

            return Distributions;
        }

        public static GUIContent[] GetPlacementModeGuiContent() {
            if (PlacementModeGUIContent == null) {
                
                PlacementModeGUIContent = GetPlacementModes()
                    .Select(p => new GUIContent(p.Name))
                    .ToArray();
            }

            return PlacementModeGUIContent;
        }

        public SelectionSettings SelectionSettings = new SelectionSettings();
        public PlacementCollection PlacementCollection = null;

        public Vector2 ScreenPosition;
        public Vector3 PlacementPosition;
        public Vector3 RotationPosition;

        public IPlacementMode CurrentPlacementMode;
        public BrushBase CurrentBrush;
        public DistributionBase CurrentDistribution;

        private bool IsHidden;

        private readonly RaycastHit[] RaycastHitsCache = new RaycastHit[128];

        public bool HasPlacementSelection = false;

        public void OnEnable()
        {
            CurrentPlacementMode = GetPlacementModes()[SelectionSettings.PlacementModeIndex];
            CurrentBrush = GetBrushMapping()[SelectionSettings.SelectedBrushIndex];
            CurrentDistribution = GetDistributionModes()[SelectionSettings.SelectedDistributionIndex];
        }

        public void NotifyChange()
        {
            CurrentBrush = GetBrushMapping()[SelectionSettings.SelectedBrushIndex];
            CurrentBrush.OnSelected(this);
            CurrentDistribution = GetDistributionModes()[SelectionSettings.SelectedDistributionIndex];
            GeneratePlacementInformation(SelectionSettings, ScreenPosition, PlacementPosition);
        }

        public void ClearSelection()
        {
            if(CurrentBrush != null) {
                CurrentBrush.Rotation = 0f;
                CurrentBrush.OnClearSelection(this);
            }

            PreviousSelection = new List<PaletteItem>(SelectionSettings.SelectedItems);

            SelectionSettings.ClearSelection();
            DestroyPlacementObjects();
            PlacementCollection = null;
            IsHidden = true;
        }

        public void RestoreSelection()
        {
            IsHidden = false;
            Brushes[SelectionSettings.SelectedBrushIndex].OnSelected(this);
            if (PreviousSelection == null || PreviousSelection.Count == 0) {
                return;
            }

            SelectionSettings.SelectedItems = new List<PaletteItem>(PreviousSelection);
            GeneratePlacement();
        }

        public bool IsCleared() => (PlacementCollection?.Placements?.Count ?? 0) == 0;

        public Vector3? GetInWorldPoint(Ray ray)
        {
            var hitCount = Physics.RaycastNonAlloc(ray, RaycastHitsCache, 10000, int.MaxValue, QueryTriggerInteraction.Ignore);
            return CurrentPlacementMode.IsValidPlacement(RaycastHitsCache, hitCount, PlacementCollection)?.point;
        }

        public void MovePosition(Vector2 screenPosition, Vector3 position)
        {
            UnhidePlacement();

            ScreenPosition = screenPosition;
            PlacementPosition = position;

            // Avoids crashes if the data is lost due to recompilation
            if (SelectionSettings == null) {
                ClearSelection();
                return;
            }

            CurrentBrush.MoveBrush(PlacementPosition);
            UpdatePlacements();
        }

        public void UpdatePlacements()
        {
            if (PlacementCollection == null) {
                return;
            }

            foreach (var placementInformation in PlacementCollection.Placements) {
                if (placementInformation.GameObject == null) {
                    continue;
                }

                placementInformation.GameObject.SetActive(true);
                var individualPosition = CurrentBrush.BrushPosition + placementInformation.Offset;
                var individualRotation = placementInformation.Rotation;

                var raycastHit = GetTerrainRaycastHit(individualPosition);

                if (placementInformation.Item.AdvancedOptions.UseIndividualGroundHeight || SelectionSettings.OrientToNormal) {
                    if (raycastHit.HasValue) {
                        if (placementInformation.Item.AdvancedOptions.UseIndividualGroundHeight) {
                            individualPosition.y = raycastHit.Value.point.y + placementInformation.Offset.y;
                        }

                        if (SelectionSettings.OrientToNormal) {
                            individualRotation = Quaternion.LookRotation(raycastHit.Value.normal, Vector3.back) * Quaternion.Euler(90, 0, 0) * placementInformation.Rotation;
                        }
                    }
                }

                placementInformation.GameObject.transform.position = individualPosition;
                placementInformation.GameObject.transform.rotation = individualRotation;
                if (raycastHit.HasValue) {
                    placementInformation.GameObject.SetActive(true);
                } else {
                    placementInformation.GameObject.SetActive(false);
                }
            }
        }
        private void GeneratePlacementInformation(SelectionSettings settings, Vector2 screenPosition, Vector3 worldPosition)
        {
            DestroyPlacementObjects();

            PlacementCollection = CurrentBrush.GeneratePlacementForBrush(worldPosition, settings);
            RotatatePlacement(CurrentBrush.Rotation);

            if(IsHidden) {
                PlacementCollection.Hide();
            } else {
                MovePosition(ScreenPosition, worldPosition);
            }
        }

        public void DestroyPlacementObjects()
        {
            if (PlacementCollection == null) {
                return;
            }

            foreach (var placementInformationObject in PlacementCollection.Placements) {
                GameObject.DestroyImmediate(placementInformationObject.GameObject);
            }
            HasPlacementSelection = false;
        }

        private RaycastHit? GetTerrainRaycastHit(Vector3 position)
        {
            var ray = new Ray(position + Vector3.up * 1000, Vector3.down);
            var hitCount = Physics.RaycastNonAlloc(ray, RaycastHitsCache, 3000);

            return CurrentPlacementMode.IsValidPlacement(RaycastHitsCache, hitCount, PlacementCollection);
        }

        public float RotateTowardsPosition(Vector3 position)
        {
            if (PlacementCollection == null || !PlacementCollection.HasItems()) {
                return CurrentBrush.Rotation;
            }

            PlacementCollection.RotateTowardsPosition(position);
            return (CurrentBrush.BrushPosition - position).DirectionToRotationY();
        }

        public float RotatatePlacement(Vector3 position)
        {
            if (PlacementCollection == null || !PlacementCollection.HasItems()) {
                return CurrentBrush.Rotation;
            }

            var rotation = (CurrentBrush.BrushPosition - position).DirectionToRotationY();
            RotatatePlacement(rotation);
            return rotation;
        }

        public void RotatatePlacement(float rotation)
        {
            PlacementCollection.RotatePlacement(rotation);
            UpdatePlacements();
        }

        public void StartPlacement(Vector2 screenPosition)
        {
            ScreenPosition = screenPosition;
            CurrentBrush.StartPlacement(PlacementPosition, this);
        }

        public void ActiveDragPlacement(Vector3 worldPosition, double deltaTime)
        {
            CurrentBrush.ActiveDragPlacement(worldPosition, SelectionSettings, deltaTime, this);
        }

        public void ShiftDragPlacement(Vector3 worldPosition, double deltaTime)
        {
            CurrentBrush.ShiftDragPlacement(worldPosition, SelectionSettings, deltaTime, this);
        }

        public void PassiveDragPlacement(Vector3 worldPosition, double deltaTime)
        {
            CurrentBrush.StaticDragPlacement(worldPosition, SelectionSettings, deltaTime, this);
        }

        public void EndPlacement()
        {
            var placedGameObjects = CurrentBrush.EndPlacement(PlacementPosition, CurrentPlacementMode.ParentObject, SelectionSettings, this);

            if(placedGameObjects.Count == 0) {
                return;
            }

            foreach (var placedObject in placedGameObjects) {
                Undo.RegisterCreatedObjectUndo(placedObject, "Placed object");
                SpawnEffects.RegisterObject(placedObject);
            }
        }

        public void GeneratePlacement()
        {
            GeneratePlacementInformation(SelectionSettings, ScreenPosition, PlacementPosition);
            HasPlacementSelection = true;
        }

        public void DrawBrushHandle(Vector3 adjustedInworldPosition) {  
            if(IsHidden) {
                return;
            }

            CurrentBrush.DrawBrushHandle(PlacementPosition, adjustedInworldPosition);
            CurrentBrush.DrawSceneHandleText(ScreenPosition, PlacementPosition, this);
        }

        public void HidePlacement() {
            if (PlacementCollection != null) {
                PlacementCollection.Hide();
            }

            IsHidden = true;
        }

        public void UnhidePlacement() {
            if(!IsHidden) {
                return;
            }

            if (PlacementCollection != null) {
                PlacementCollection.Show();
            }

            IsHidden = false;
        }
    }
}