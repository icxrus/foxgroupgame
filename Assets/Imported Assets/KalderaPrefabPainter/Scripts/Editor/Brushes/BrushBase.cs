using CollisionBear.WorldEditor.Lite.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Brushes
{
    public abstract class BrushBase
    {
        private const float CompassRadius = 5f;
        private static readonly Vector3 CompassScale = new Vector3(CompassRadius, CompassRadius, CompassRadius);

        private const float MinScaleFactor = 0.05f;
        private const float MaxScaleFactor = 10f;

        private const float ScaleUpStepValue = 1.01f;
        private const float ScaleDownStepValue = 0.990099f;

        private static readonly Vector3[] LineMesh = new Vector3[] {
            new Vector3(-0.5f, 0, 0),
            new Vector3(0.5f, 0, 0),
            new Vector3(0.5f, 0, -1),
            new Vector3(-0.5f, 0, -1),
        };

        protected const float ArrowSize = 3f;
        protected static readonly Vector3 ArrowScale = new Vector3(ArrowSize, ArrowSize, ArrowSize);

        protected static readonly Quaternion QuaterRotation = Quaternion.Euler(0, 90, 0);

        protected const float DirectionSnapping = 22.5f;

        protected const string FloatFormat = "0.00";
        protected const string RotationFormat = "0";

        protected static readonly Vector2 InitialOffset = new Vector2(0, 50);
        protected static readonly Vector2 TextOffset = new Vector2(0, 16);

        protected static readonly Collider[] CastCollidersCache = new Collider[64];
        protected static readonly List<Vector2> EmptyPointList = new List<Vector2>();
        protected static readonly List<GameObject> EmptyGameObjectList = new List<GameObject>();

        protected static readonly Color HandleOutlineColor = new Color(1f, 0.5f, 0.8f, 0.8f);
        protected static readonly Color HandleBrushColor = new Color(1f, 0.5f, 0.8f, 0.1f);

        public readonly int Index;

        public Vector3 BrushPosition;
        public float Rotation;
        public float ScaleFactor = 1f;

        protected Vector3? StartDragPosition;
        protected Vector3? EndDragPosition;
        protected Quaternion? LastRotation = null;

        public abstract string Name { get; }
        public abstract KeyCode HotKey { get; }

        protected abstract string ToolTip { get; }
        protected abstract string ButtonImagePath { get; }

        private GUIContent ButtonContent;

        public BrushBase(int index)
        {
            Index = index;
        }

        public GUIContent GetButtonContent() 
        {
            if(ButtonContent == null) {
                ButtonContent = LoadGUIContent();
            }

            return ButtonContent;
        }

        public virtual bool Disabled { get; } = false;
        public virtual bool ShowBrush(ScenePlacer placer) => !placer.IsCleared();

        public virtual void OnSelected(ScenePlacer placer) { }
        public virtual void OnClearSelection(ScenePlacer placer) { }
        public abstract void DrawBrushEditor(ScenePlacer placer);
        public virtual void DrawAdditionalSettings(ScenePlacer placer, SelectionSettings settings)
        {
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope()) {
                EditorCustomGUILayout.SetGuiBackgroundColorState(settings.ParentObjectsToBaseObject);
                if (GUILayout.Button(KalderaEditorUtils.ParentObjectToBaseObjectContent, GUILayout.Width(KalderaEditorUtils.IconButtonSize), GUILayout.Height(KalderaEditorUtils.IconButtonSize))) {
                    settings.ParentObjectsToBaseObject = !settings.ParentObjectsToBaseObject;
                }

                //EditorCustomGUILayout.SetGuiBackgroundColorState(settings.AllowCollisions);
                //if (GUILayout.Button(WorldEditorUtils.AllowCollisionContent, GUILayout.Width(WorldEditorUtils.IconButtonSize), GUILayout.Height(WorldEditorUtils.IconButtonSize))) {
                //    settings.AllowCollisions = !settings.AllowCollisions;
                //}

                EditorCustomGUILayout.SetGuiBackgroundColorState(settings.OrientToNormal);
                if (GUILayout.Button(KalderaEditorUtils.OrientToNormalContent, GUILayout.Width(KalderaEditorUtils.IconButtonSize), GUILayout.Height(KalderaEditorUtils.IconButtonSize))) {
                    settings.OrientToNormal = !settings.OrientToNormal;
                }

                EditorCustomGUILayout.RestoreGuiColor();
            }

            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField(KalderaEditorUtils.ObjectLimitContent, GUILayout.Width(KalderaEditorUtils.OptionLabelWidth));
                var tmpObjectLimit = EditorGUILayout.IntSlider(settings.ObjectLimit, KalderaEditorUtils.ObjectLimitMin, KalderaEditorUtils.ObjectLimitMax);
                if (tmpObjectLimit != settings.ObjectLimit) {
                    settings.ObjectLimit = tmpObjectLimit;
                    placer.NotifyChange();
                }
            }
        }

        public virtual void DrawSceneHandleText(Vector2 screenPosition, Vector3 worldPosition, ScenePlacer placer)
        {
            DrawHandleTextAtOffset(screenPosition, 0, KalderaEditorUtils.MousePlaceToolTip);
        }

        public virtual bool HandleKeyEvents(Event keyEvent, ScenePlacer placer) => false;
        public virtual bool CanPlaceObject(Vector3 position, PlacementCollection placement) => placement.HasItems();

        public virtual void StartPlacement(Vector3 position, ScenePlacer placer) 
        {
            StartDragPosition = position;
        }

        public virtual void MoveBrush(Vector3 position)
        {
            BrushPosition = position;
        }

        public virtual void ActiveDragPlacement(Vector3 position, SelectionSettings settings, double deltaTime, ScenePlacer placer)
        {
            EndDragPosition = position;
            BrushPosition = position;
        }

        public virtual void StaticDragPlacement(Vector3 position, SelectionSettings settings, double deltaTime, ScenePlacer placer)
        {
            EndDragPosition = position;
        }

        public virtual void ShiftDragPlacement(Vector3 position, SelectionSettings settings, double deltaTime, ScenePlacer placer)
        {
            // Defaults to the same as an active drag
            ActiveDragPlacement(position, settings, deltaTime, placer);
        }

        public virtual List<GameObject> EndPlacement(Vector3 position, GameObject parentCollider, SelectionSettings settings, ScenePlacer placer)
        {
            if (!CanPlaceObject(position, placer.PlacementCollection)) {
                return EmptyGameObjectList;
            }

            return PlaceObjects(position, parentCollider, settings, placer);
        }

        public virtual PlacementCollection GeneratePlacementForBrush(Vector3 position, SelectionSettings selectionSettings)
        {
            var result = new PlacementCollection();

            if (!selectionSettings.HasItems()) {
                return result;
            }

            var validItems = selectionSettings.GetItemsWithVariants();
            if (validItems.Count == 0) {
                return result;
            }

            UpdatePlacementPoints(position, selectionSettings, result);
            return result;
        }

        public virtual void DrawBrushHandle(Vector3 placementPosition, Vector3 mousePosition) { }

        protected Vector3 ScreenToWorldPosition(Vector2 screenPosition) => HandleUtility.GUIPointToWorldRay(screenPosition).GetPoint(0.1f);

        protected virtual GUIContent LoadGUIContent() {
            var image = KalderaEditorUtils.LoadAssetPath(ButtonImagePath);
            return new GUIContent(image, Name + "\n" + ToolTip + "\nShft + " + HotKey);
        }

        protected bool HasDrag(Vector3? startPosition, Vector3? endPosition)
        {
            if (!startPosition.HasValue || !endPosition.HasValue) {
                return false;
            }

            var startValue = startPosition.Value;
            var endValue = endPosition.Value;

            startValue.y = 0;
            endValue.y = 0;

            return (startValue - endValue).sqrMagnitude > 0.01f;
        }

        protected void DrawRotationCompass(Vector3 startPosition, Vector3 worldPosition)
        {
            worldPosition.y = startPosition.y;
            var direction = (worldPosition - startPosition).normalized;
            if(direction.sqrMagnitude < 0.0001f) {
                direction = Vector3.forward;
            }

            var rotation = Quaternion.LookRotation(direction) * QuaterRotation;
            DrawOnSceneViewMesh(KalderaEditorUtils.PlaneMesh, KalderaEditorUtils.CompassMaterial, startPosition, rotation, CompassScale);
        }

        protected void DrawRotationArrow(Vector3 startPosition, Vector3 worldPosition)
        {
            var rotation = Quaternion.LookRotation((worldPosition - startPosition).normalized) * QuaterRotation;
            DrawOnSceneViewMesh(KalderaEditorUtils.PlaneMesh, KalderaEditorUtils.ArrowMaterial, worldPosition, rotation, ArrowScale);
        }

        protected void DrawLineArrow(Vector3 startPosition, Vector3 worldPosition)
        {
            var rotation = Quaternion.LookRotation((worldPosition - startPosition).normalized) * QuaterRotation;
            DrawOnSceneViewMesh(KalderaEditorUtils.PlaneMesh, KalderaEditorUtils.ArrowMaterial, startPosition - (rotation * Vector3.left * ArrowSize * 0.75f), rotation, new Vector3(ArrowSize, ArrowSize, ArrowSize));

            DrawOnSceneViewMesh(KalderaEditorUtils.PlaneMesh, KalderaEditorUtils.LongArrowMaterial, worldPosition - (rotation * Vector3.left * ArrowSize * 0.5f), rotation, new Vector3(ArrowSize, ArrowSize, ArrowSize));
        }

        protected void DrawOnSceneViewMesh(Mesh mesh, Material material, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if(mesh == null || material == null) {
                return;
            }

            var bodyTrs = Matrix4x4.TRS(position, rotation, scale);

            for (int i = 0; i < material.passCount; i++) {
                if (material.SetPass(i)) {
                    Graphics.DrawMeshNow(mesh, bodyTrs);
                }
            }
        }

        protected void DrawLine(Vector3 position, Vector3 direction, float width, float length, Color color)
        {
            Handles.color = color;
            var bodyTrs = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(direction), new Vector3(width, width, length));
            var bodyCorners = LineMesh
                .Select(v => position + bodyTrs.MultiplyVector(v))
                .ToArray();

            Handles.DrawAAConvexPolygon(bodyCorners);
        }

        protected void DrawHandleTextAtOffset(Vector2 screenPosition, int textIndex, GUIContent content)
        {
            var offset = new Vector2(50, 0);
            Handles.Label(ScreenToWorldPosition(screenPosition + InitialOffset + (TextOffset * textIndex) - offset), content, StylesUtility.HandleTextStyle);
        }

        protected Vector3 GetRotationSnappedPosition(Vector3 start, Vector3 end)
        {
            var drag = end - start;
            var angle = Vector3.Angle(Vector3.forward, drag.normalized);

            // We're on the other side
            if(drag.x < 0) {
                angle = -angle;
            }

            angle = Mathf.Round(angle / DirectionSnapping) * DirectionSnapping;
            var distance = drag.magnitude;

            return start + (Quaternion.Euler(0, angle, 0) * Vector3.forward * distance);
        }

        protected virtual void UpdatePlacementPoints(Vector3 position, SelectionSettings selectionSettings, PlacementCollection placementCollection)
        {
            var placementPoints = GetPlacementOffsetValues(position, selectionSettings)
                .Take(selectionSettings.ObjectLimit)
                .DefaultIfEmpty(Vector2.zero);

            foreach (var point in placementPoints) {
                var placementInformation = CreatePlacementInformation(position, point, selectionSettings);
                placementCollection.Placements.Add(placementInformation);
            }
        }

        protected virtual List<PlacementInformation> PlacementsToPlace(ScenePlacer placer) => placer.PlacementCollection.Placements;

        protected List<GameObject> PlaceObjects(Vector3 position, GameObject parentObject, SelectionSettings settings, ScenePlacer placer)
        {
            var placements = PlacementsToPlace(placer);
            foreach (var placementInformation in placements) {
                var collider = placementInformation.GameObject.GetComponent<Collider>();
                if (collider != null) {
                    collider.enabled = false;
                }
            }

            var colliderStatus = new Dictionary<Collider, bool>();

            var result = new List<GameObject>();
            foreach (var placementInformation in placements) {
                if (!placementInformation.GameObject.activeSelf) {
                    continue;
                }

                var placedGameObject = PlaceObject(placementInformation, position, parentObject, settings, colliderStatus);
                if (placedGameObject != null) {
                    result.Add(placedGameObject);
                }
            }

            foreach (var key in colliderStatus.Keys) {
                key.enabled = colliderStatus[key];
            }

            return result;
        }

        private GameObject PlaceObject(PlacementInformation placementInformation, Vector3 position, GameObject parentObject, SelectionSettings settings, Dictionary<Collider, bool> colliderStatus)
        {
            var result = (GameObject)PrefabUtility.InstantiatePrefab(placementInformation.PrefabObject);

            if (placementInformation.Item.AdvancedOptions.NameType == AdvancedOptions.ItemNameType.UseItemName) {
                result.name = placementInformation.Item.Name;
            } else {
                result.name = placementInformation.GameObject.name;
            }

            result.transform.position = placementInformation.GameObject.transform.position;
            result.transform.rotation = placementInformation.GameObject.transform.rotation;
            result.transform.localScale = placementInformation.Scale * ScaleFactor;

            var placementAllowed = true;

            if (!placementInformation.Item.AdvancedOptions.AllowCollision && !settings.AllowCollisions) {
                var collider = result.GetComponent<Collider>();
                placementAllowed = AllowPlacement(collider, result, placementAllowed, parentObject);
            }

            if (placementAllowed) {
                SetParentObject(result, parentObject, settings);
                var collider = result.GetComponent<Collider>();
                if (collider != null) {
                    colliderStatus.Add(collider, collider.enabled);
                    collider.enabled = false;
                }

                return result;
            } else {
                GameObject.DestroyImmediate(result);
                return null;
            }
        }

        public virtual void CycleVariant(int steps, ScenePlacer placer)
        {
            // Default implemetation is to randomize the placement
            placer.GeneratePlacement();
        }

        public virtual void ScaleBrush(float delta, ScenePlacer placer)
        {
            ScaleFactor = Mathf.Clamp(ScaleFactor * DeltaScale(delta), MinScaleFactor, MaxScaleFactor);

            foreach (var placement in placer.PlacementCollection.Placements) {
                var validObjects = placement.Item.ValidObjects();
                var selectedVariantIndex = validObjects.IndexOf(placement.PrefabObject);
                placement.ReplacePlacementObject(selectedVariantIndex, BrushPosition, ScaleFactor);
            }
        }

        private float DeltaScale(float delta)
        {
            if(delta > 0) {
                return Mathf.Pow(ScaleUpStepValue, delta);
            } else if(delta < 0) {
                return Mathf.Pow(ScaleDownStepValue, Mathf.Abs(delta));
            } else {
                return 1f;
            }
        }

        protected PlacementInformation CreatePlacementInformation(Vector3 position, Vector3 pointOffset, SelectionSettings selectionSettings)
        {
            var item = GetRandomItem(selectionSettings.SelectedItems);

            var offset = new Vector3(pointOffset.x, 0, pointOffset.y);
            var result = GetPlacementInformation(position, item, offset, item.GetObjectVariant());
            result.CreatePlacementGameObject(position, ScaleFactor);

            return result;
        }

        protected virtual Vector3 GetItemRotation(Vector3 position, PaletteItem item, GameObject prefabObject) => item.GetRotation(prefabObject);

        protected abstract List<Vector2> GetPlacementOffsetValues(Vector3 position, SelectionSettings selectionSettings);

        private PaletteItem GetRandomItem(List<PaletteItem> selectedItems) => selectedItems[Random.Range(0, selectedItems.Count)];

        private PlacementInformation GetPlacementInformation(Vector3 position, PaletteItem item, Vector3 offset, GameObject prefabObject)
        {
            var eulerRotation = GetItemRotation(position, item, prefabObject);
            var localScale = GetItemScale(item, prefabObject);

            return new PlacementInformation(item, prefabObject, offset, eulerRotation, localScale);
        }

        private Vector3 GetItemScale(PaletteItem item, GameObject prefabObject)
        {
            var result = item.GetScale();

            if (item.AdvancedOptions.MultiplyPrefabScale) {
                result.x *= prefabObject.transform.localScale.x;
                result.y *= prefabObject.transform.localScale.y;
                result.z *= prefabObject.transform.localScale.z;
            }

            return result;
        }

        private void SetParentObject(GameObject placedObject, GameObject parentObject, SelectionSettings settings)
        {
            if (parentObject != null && settings.ParentObjectsToBaseObject) {
                placedObject.transform.parent = parentObject.transform;
            }
        }

        private bool AllowPlacement(Collider collider, GameObject tmpGameObject, bool initialState, GameObject placementParentObject)
        {
            return true;

            // TODO: Reenable this when we figure out how to do it in a convinient manner
            //if (collider == null) {
            //    return initialState;
            //}

            //if (!(collider is BoxCollider || collider is SphereCollider)) {
            //    return initialState;
            //}

            //var hitCount = collider.CastNonAlloc(CastCollidersCache);
            //for (var i = 0; i < hitCount; i++) {
            //    if (CastCollidersCache[i].gameObject != placementParentObject && CastCollidersCache[i].gameObject != tmpGameObject) {
            //        return false;
            //    }
            //}

            //return initialState;
        }
    }
}