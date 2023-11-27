using CollisionBear.WorldEditor.Lite.Brushes;
using CollisionBear.WorldEditor.Lite.Extensions;
using CollisionBear.WorldEditor.Lite.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CollisionBear.WorldEditor.Lite
{
    public class PaletteWindow : EditorWindow
    {
        const float LeftOffset = 2;
        const float ButtonPadding = 3;
        const float RightOffset = 24;
        const float ScrollBarWidth = 16;

        static readonly Vector2 WindowMinSize = new Vector2(200, 150);

        [MenuItem(KalderaEditorUtils.WindowBasePath)]
        public static void ShowWindow()
        {
            var window = GetWindow<PaletteWindow>();
            window.minSize = WindowMinSize;
            window.titleContent = KalderaEditorUtils.TitleGuiContent;
            window.ShowUtility();
        }

        public static void RefreshAllWindows() {
            foreach (var window in Resources.FindObjectsOfTypeAll<PaletteWindow>()) {
                window.UpdateAvailableAssets();
                window.Repaint();
            }
        }

        [SerializeField]
        private SelectableAsset SelectedAsset;

        [SerializeField]
        private Palette SelectedPalette;

        [SerializeField]
        private ScenePlacer ScenePlacer = new ScenePlacer();

        [SerializeField]
        public SelectionSettings SelectionSettings = new SelectionSettings();

        [SerializeField]
        private Vector2 CurrentWindowScroll;

        private Dictionary<KeyCode, Palette> ShortKeysIndexMapping;

        [SerializeField]
        private bool IsPlacementModeHintsOpen;

        [SerializeField]
        private List<SelectableAsset> AvailableAssets = new List<SelectableAsset>();

        [SerializeField]
        private GUIContent[] AvailableAssetGuiContent = new GUIContent[0];

        [SerializeField]
        private List<Palette> AvailablePalettes = new List<Palette>();

        [SerializeField]
        private GUIContent[] AvailablePalettesGuiContent = new GUIContent[0];

        private bool IsMousePressed = false;
        private double LastTimestamp;

        void OnEnable()
        {
            wantsMouseEnterLeaveWindow = true;

            if (ScenePlacer == null) {
                ScenePlacer = new ScenePlacer();
            }

            ScenePlacer.OnEnable();

            LastTimestamp = EditorApplication.timeSinceStartup;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            SceneView.duringSceneGui += OnSceneView;
            EditorSceneManager.sceneClosing += SceneChange;
            AssemblyReloadEvents.afterAssemblyReload += ClearSelection;

            UpdateAvailableAssets();
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneView;
            EditorSceneManager.sceneClosing -= SceneChange;
            AssemblyReloadEvents.afterAssemblyReload -= ClearSelection;
        }

        public void UpdateAvailableAssets()
        {
            UpdateShortKeys();
            AvailableAssets = GetAvailableSelectableAssets();
            AvailableAssetGuiContent = GetSelectableAssetsContent(AvailableAssets);
            
            // TODO: Repeated code from 412. Unify
            if(SelectedAsset is PaletteSet paletteSet) {
                AvailablePalettes = paletteSet.Categories
                .Where(c => c != null)
                .ToList();

            AvailablePalettesGuiContent = AvailablePalettes
                .Select(c => new GUIContent(c.name))
                .ToArray();
            }

            if(!AvailablePalettes.Contains(SelectedPalette)) {
                if(AvailablePalettes.Count == 0) {
                    SelectedPalette = null;
                } else {
                    SelectedPalette = AvailablePalettes[0];
                }
            }
        }

        protected void SceneChange(Scene scene, bool removingScene)
        {
            ClearSelection();
        }

        protected void SelectionChanged()
        {
            ClearSelection();
        }

        protected bool IsSelectableAsset(SelectableAsset selectedAsset)
        {
            if (selectedAsset == null || !(selectedAsset is PaletteSet)) {
                return false;
            }

            var selectedAssetSet = (PaletteSet)SelectedAsset;
            if (selectedAssetSet.Categories == null) {
                return false;
            }

            return true;
        }

        protected void UpdateShortKeys()
        {
            ShortKeysIndexMapping = new Dictionary<KeyCode, Palette>();

            if (!IsSelectableAsset(SelectedAsset)) {
                return;
            }

            var selectedAssetSet = (PaletteSet)SelectedAsset;
            for (int i = 0; i < selectedAssetSet.Categories.Count; i++) {
                var category = selectedAssetSet.Categories[i];
                if (category == null) {
                    continue;
                }

                if (category.ShortKey != KeyCode.None && !ShortKeysIndexMapping.ContainsKey(category.ShortKey)) {
                    ShortKeysIndexMapping.Add(category.ShortKey, selectedAssetSet.Categories[i]);
                }
            }
        }

        public void ClearSelection()
        {
            ScenePlacer.ClearSelection();
        }

        public void OnSceneView(SceneView sceneView)
        {
            sceneView.Repaint();

            Event currentEvent = Event.current;

            if (!ScenePlacer.CurrentBrush.ShowBrush(ScenePlacer)) {
                if(IsSpaceKey(currentEvent)) {
                    ScenePlacer.RestoreSelection();
                    currentEvent.Use();
                    Repaint();
                }

                return;
            }

            if (ScenePlacer.CurrentBrush == null) {
                ClearSelection();
                return;
            }

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            var adjustedInworldPosition = ScenePlacer.GetInWorldPoint(currentEvent.GUIPointToRay());
            if (adjustedInworldPosition.HasValue) {
                ScenePlacer.DrawBrushHandle(adjustedInworldPosition.Value);
            }

            HandleInput(currentEvent, adjustedInworldPosition); 
            HandleShortCuts(currentEvent);
        }

        private void HandleInput(Event currentEvent, Vector3? adjustedInworldPosition)
        {
            if (currentEvent.type == EventType.Layout) {
                return;
            } else if(currentEvent.type == EventType.MouseEnterWindow) {
                ScenePlacer.UnhidePlacement();
                return;
            } else if (currentEvent.type == EventType.MouseLeaveWindow) {
                // Ensures any drag motions is stopped if the mouse pointer leaves Unity's editor window
                IsMousePressed = false;
                ScenePlacer.HidePlacement();
                return;
            } else 

            if(currentEvent.isMouse && currentEvent.button == 0) {
                if(currentEvent.type == EventType.MouseDown) {
                    IsMousePressed = true;
                } else if(currentEvent.type == EventType.MouseUp) { 
                    IsMousePressed = false;
                }
            }

            if (!adjustedInworldPosition.HasValue) {
                return;
            }

            HandleToolInput(currentEvent, adjustedInworldPosition.Value);
        }
        
        private void HandleToolInput(Event currentEvent, Vector3 adjustedInworldPosition)
        {
            if(currentEvent.type == EventType.Repaint) {
                return;
            }

            var deltaTime = EditorApplication.timeSinceStartup - LastTimestamp;
            LastTimestamp = EditorApplication.timeSinceStartup;

            // Resumes brush after placement
            if (currentEvent.isMouse) {
                if(!ScenePlacer.HasPlacementSelection) {
                    ScenePlacer.GeneratePlacement();
                    ScenePlacer.MovePosition(currentEvent.mousePosition, adjustedInworldPosition);
                }
            }

            // Start placement 
            if (!currentEvent.shift && currentEvent.isMouse && currentEvent.IsPureMouseStartClick()) {
                ScenePlacer.StartPlacement(currentEvent.mousePosition);
                currentEvent.Use();
                return;
            }

            // Active drag
            if (!currentEvent.shift && currentEvent.isMouse && currentEvent.IsPureMouseDrag()) {
                ScenePlacer.ActiveDragPlacement(adjustedInworldPosition, deltaTime);
                ScenePlacer.UpdatePlacements();
                currentEvent.Use();
                return;
            }

            // Toggling shift key
            if (currentEvent.keyCode == KeyCode.LeftShift && IsMousePressed && !currentEvent.alt && !currentEvent.control) {
                if (currentEvent.type == EventType.KeyDown) {
                    ScenePlacer.ShiftDragPlacement(adjustedInworldPosition, deltaTime);
                    ScenePlacer.UpdatePlacements();
                    currentEvent.Use();
                    return;
                } else if (currentEvent.type == EventType.KeyUp) {
                    ScenePlacer.ActiveDragPlacement(adjustedInworldPosition, deltaTime);
                    ScenePlacer.UpdatePlacements();
                    currentEvent.Use();
                    return;
                }
            }

            // Passive drag 
            if (!currentEvent.shift && IsMousePressed && currentEvent.NoModifiers()) {
                ScenePlacer.PassiveDragPlacement(adjustedInworldPosition, deltaTime);
                currentEvent.Use();
                return;
            }

            // Shift drag
            if(currentEvent.shift && IsMousePressed && !currentEvent.alt && !currentEvent.control) {
                ScenePlacer.ShiftDragPlacement(adjustedInworldPosition, deltaTime);
                currentEvent.Use();
                return;
            }

            // End placement
            if (currentEvent.isMouse && currentEvent.IsPureMouseEndClick()) {
                ScenePlacer.EndPlacement();
                ScenePlacer.DestroyPlacementObjects();
                currentEvent.Use();
                return;
            }

            // Cycle prefab variant tool 
            if (currentEvent.shift && currentEvent.isScrollWheel) {
                ScenePlacer.CurrentBrush.CycleVariant(GetMouseWheelDeltaSteps((int)currentEvent.delta.y), ScenePlacer);
                ScenePlacer.MovePosition(currentEvent.mousePosition, ScenePlacer.PlacementPosition);
                currentEvent.Use();
                return;
            }

            // Scale tool 
            if (currentEvent.control && currentEvent.isScrollWheel) {
                ScenePlacer.CurrentBrush.ScaleBrush(-currentEvent.delta.y, ScenePlacer);
                ScenePlacer.UpdatePlacements();
                currentEvent.Use();
                return;
            }

            // Clear 
            if (IsSpaceKey(currentEvent)) {
                ScenePlacer.ClearSelection();
                currentEvent.Use();
                Repaint();
                return;
            }

            // Move
            if (currentEvent.NoModifiers()) {
                ScenePlacer.MovePosition(currentEvent.mousePosition, adjustedInworldPosition);
            }
        }

        private bool IsSpaceKey(Event currentEvent) => currentEvent.isKey && currentEvent.keyCode == KeyCode.Space && currentEvent.type == EventType.KeyDown;

        private int GetMouseWheelDeltaSteps(int steps) => Mathf.Clamp(steps, -1, 1);

        private void OnGuiClearBrush(Event currentEvent)
        {
            // Clear 
            if (currentEvent.isKey && currentEvent.keyCode == KeyCode.Space) {
                ScenePlacer.ClearSelection();
                currentEvent.Use();
                return;
            }
        }

        private void HandleShortCuts(Event currentEvent)
        {
            if(ScenePlacer?.CurrentBrush == null) {
                return;
            }

            if (EditorGUIUtility.editingTextField) {
                return;
            }

            if (currentEvent.type == EventType.KeyDown) {
                if (currentEvent.shift) {
                    foreach (var key in ShortKeysIndexMapping.Keys) {
                        if (currentEvent.keyCode == key) {
                            SelectedPalette = ShortKeysIndexMapping[key];
                            currentEvent.Use();
                        }
                    }

                    foreach (var brush in ScenePlacer.GetBrushMapping()) {
                        if (currentEvent.keyCode == brush.HotKey) {
                            SetBrushTypeFromHotkey(brush, currentEvent);
                        }
                    }
                }
            }

            if (ScenePlacer.CurrentBrush.HandleKeyEvents(currentEvent, ScenePlacer)) {
                currentEvent.Use();
            }
        }

        public void SetBrushTypeFromHotkey(BrushBase brush, Event currentEvent)
        {
            SetBrushType(brush);
            NotifyChange();
            currentEvent.Use();
        }

        void OnGUI()
        {
            OnGuiClearBrush(Event.current);
            HandleShortCuts(Event.current);

            DrawSelectedAsset();
            SelectedPalette = GetSelectedWindowCategory(SelectedAsset);
            EditorGUILayout.Space();
            DrawSetPlacementMode();
            EditorGUILayout.Space();
            DrawBrushTools();
            EditorGUILayout.Space();
            DrawScrollWrapper();

            DrawVersionFooter();
            DrawLiteVersionNote();
        }

        private void DrawSelectedAsset()
        {
            SetSelectedAsset(GetSelectableAsset());
        }

        private void SetSelectedAsset(SelectableAsset newSelectedAsset) {
            if(SelectedAsset == newSelectedAsset) {
                return;
            }

            SelectedAsset = newSelectedAsset;
            UpdateShortKeys();

            if(SelectedAsset == null) {
                return;
            }

            if(!(SelectedAsset is PaletteSet paletteCollection)) {
                AvailablePalettes = new List<Palette>();
                AvailablePalettesGuiContent = new GUIContent[0];
                return;
            }

            AvailablePalettes = paletteCollection.Categories
                .Where(c => c != null)
                .ToList();

            AvailablePalettesGuiContent = AvailablePalettes
                .Select(c => new GUIContent(c.name))
                .ToArray();
        }

        private void DrawSetPlacementMode()
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                using (new EditorGUILayout.HorizontalScope()) {
                    var placementModeIndex = EditorGUILayout.Popup(KalderaEditorUtils.SelectPlacementModeContent, ScenePlacer.SelectionSettings.PlacementModeIndex, ScenePlacer.GetPlacementModeGuiContent());
                    if (placementModeIndex != ScenePlacer.SelectionSettings.PlacementModeIndex) {
                        SetPlacementMode(placementModeIndex);
                    }


                    EditorGUILayout.LabelField(KalderaEditorUtils.MoreInformationContent, GUILayout.Width(14));
                    IsPlacementModeHintsOpen = EditorGUILayout.Toggle(IsPlacementModeHintsOpen, StylesUtility.BoldFoldoutStyle, GUILayout.Width(14));
                }

                if(IsPlacementModeHintsOpen) {
                    using (new EditorGUILayout.VerticalScope(GUILayout.Height(KalderaEditorUtils.PlacementHintHeight))) {
                        EditorGUILayout.HelpBox($"{ScenePlacer.CurrentPlacementMode.Name}\n{ScenePlacer.CurrentPlacementMode.HintText}", MessageType.Info);
                    }
                }

                ScenePlacer.CurrentPlacementMode.DrawEditor(this);

                if (!ValidatePlacementSettings()) {
                    return;
                }
            }
        }

        private void DrawScrollWrapper()
        {
            using (var scrollScope = new EditorGUILayout.ScrollViewScope(CurrentWindowScroll)) {
                CurrentWindowScroll = scrollScope.scrollPosition;

                if (SelectedPalette == null) {
                    return;
                }

                DrawCategory(SelectedPalette);          
            }
        }

        private void DrawLiteVersionNote()
        {
            using(new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                EditorGUILayout.LabelField("This is free and trimmed down version of Kaldera Prefab Painter.");
                EditorGUILayout.LabelField("You can purchase the full version from Unity's Asset Store.");
                if(GUILayout.Button("Kaldera Prefab Painter (Full version)", EditorStyles.linkLabel)) {
                    Application.OpenURL("https://assetstore.unity.com/packages/tools/level-design/kaldera-prefab-painter-249492");
                }
            }
        }

        private List<SelectableAsset> GetAvailableSelectableAssets()
        {
            return AssetDatabase.FindAssets(string.Format("t:{0}", typeof(SelectableAsset).FullName))
                .Select(a => AssetDatabase.GUIDToAssetPath(a))
                .Select(a => AssetDatabase.LoadAssetAtPath<SelectableAsset>(a))
                .OrderBy(a => a.name)
                .ToList();
        }

        private GUIContent[] GetSelectableAssetsContent(List<SelectableAsset> selectableAssets)
        {
                return selectableAssets
                .Select(a => new GUIContent(a.GetName()))
                .ToArray();
        }

        private void SetPlacementMode(int placementModeIndex)
        {
            ScenePlacer.CurrentPlacementMode = ScenePlacer.GetPlacementModes()[placementModeIndex];
            ScenePlacer.SelectionSettings.PlacementModeIndex = placementModeIndex;
        }

        private void SetBrushType(BrushBase brush)
        {
            ScenePlacer.SelectionSettings.SelectedBrushIndex = brush.Index;
        }

        private bool ValidatePlacementSettings()
        {
            var result = true;

            if(AvailableAssets.Count == 0) {
                return false;
            }

            if (SelectedAsset == null) {
                EditorGUILayout.HelpBox(KalderaEditorUtils.SelectPaletteToolTip, MessageType.Warning);
                result = false;
            }

            var validatePlacementMessage = ScenePlacer.CurrentPlacementMode.ValidatePlacementMode();
            if (validatePlacementMessage != null) {
                EditorGUILayout.HelpBox(validatePlacementMessage, MessageType.Warning);
                result = false;
            }

            return result;
        }

        private SelectableAsset GetSelectableAsset()
        {
            if(AvailableAssets.Count == 0) {
                EditorGUILayout.HelpBox(KalderaEditorUtils.NoPaletteToolTip, MessageType.Warning);
                return null;
            }

            var currentIndex = Mathf.Max(AvailableAssets.IndexOf(SelectedAsset), 0);

            if(currentIndex >= AvailableAssetGuiContent.Length) {
                return AvailableAssets[AvailableAssetGuiContent.Length -1];
            }

            using (new EditorGUILayout.HorizontalScope()) {
                var selectedIndex = EditorGUILayout.Popup(GetPaletteSelectionContent(AvailableAssets, currentIndex), currentIndex, AvailableAssetGuiContent);
                if (GUILayout.Button(KalderaEditorUtils.ShowInProjectContent, GUILayout.Width(KalderaEditorUtils.MiniButtonWidth), GUILayout.Height(KalderaEditorUtils.MiniButtonHeight))) {
                    ShowAssetInProject(AvailableAssets[selectedIndex]);
                }

                return AvailableAssets[selectedIndex];
            }
        }

        private Palette GetSelectedWindowCategory(SelectableAsset selectedAsset)
        {
            if (selectedAsset == null) {
                using (new EditorGUI.DisabledGroupScope(true)) {
                    EditorGUILayout.LabelField(KalderaEditorUtils.SelectPaletteContent);
                }

                return null;
            } else if (selectedAsset is Palette windowCategory) {
                EditorGUILayout.LabelField(GUIContent.none);

                return windowCategory;
            } else if (selectedAsset is PaletteSet windowSet) {
                return CategoryDropdownFromSet(windowSet);
            } else {
                Debug.LogError($"Selected Palette {selectedAsset.name} is neither a Palette nor a Palette Collection");
                return null;
            }
        }

        private void ShowAssetInProject(SelectableAsset selectableAsset)
        {
            EditorGUIUtility.PingObject(selectableAsset);
            Selection.activeObject = selectableAsset;
        }

        private GUIContent GetPaletteSelectionContent(List<SelectableAsset> availableAssets, int index)
        {
            if (availableAssets[index] is Palette) {
                return KalderaEditorUtils.SelectPaletteContent;
            } else {
                return KalderaEditorUtils.SelectPaletteCollectionContent;
            }
        }

        protected Palette CategoryDropdownFromSet(PaletteSet paletteCollection)
        {
            if (AvailablePalettes.Count == 0) {
                EditorGUILayout.HelpBox(KalderaEditorUtils.EmptySetTooltip, MessageType.Warning);
                return null;
            }

            var currentItem = 0;
            if (SelectedPalette != null) {
                currentItem = AvailablePalettes.IndexOf(SelectedPalette);
            }

            using (new EditorGUILayout.HorizontalScope()) {
                var currentIndex = Mathf.Max(0, EditorGUILayout.Popup(KalderaEditorUtils.PaletteLabelContent, currentItem, AvailablePalettesGuiContent));
                if (GUILayout.Button(KalderaEditorUtils.ShowInProjectContent, GUILayout.Width(KalderaEditorUtils.MiniButtonWidth), GUILayout.Height(KalderaEditorUtils.MiniButtonHeight))) {
                    ShowAssetInProject(AvailablePalettes[currentIndex]);
                }

                return AvailablePalettes[currentIndex];
            }
        }

        protected void DrawCategory(Palette palette)
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                EditorGUILayout.LabelField(KalderaEditorUtils.PaletteLabelContent, EditorStyles.boldLabel);
                if (palette.HasAnyGroupWithItems()) {
                    foreach (var group in palette.Groups) {
                        DrawGroup(palette, group);
                    }
                    EditorGUILayout.Space();
                } else {
                    EditorGUILayout.LabelField("Palette contains no groups with items in them.", EditorStyles.miniBoldLabel);
                    if (GUILayout.Button("Start create and populate groups for this palette", EditorStyles.miniButton)) {
                        EditorGUIUtility.PingObject(SelectedPalette);
                        Selection.activeObject = SelectedPalette;
                    }
                    EditorGUILayout.Space();
                }

                EditorGUILayout.LabelField("Hold down shift to select/deselect several prefabs", EditorStyles.miniLabel);

            }
        }

        protected void DrawGroup(Palette palette, PaletteGroup group)
        {
            var windowWidth = GetWindowWidth();
            var buttonsInRow = GetButtonCountInRow(windowWidth, KalderaEditorUtils.IconButtonSize);

            var totalIndex = 0;
            var currentRow = 0;

            EditorGUILayout.LabelField(GetGroupName(group));

            while (totalIndex < group.Items.Count) {
                using (new GUI.GroupScope(GUILayoutUtility.GetRect(windowWidth, KalderaEditorUtils.IconButtonSize))) {
                    for (int rowIndex = 0; rowIndex < buttonsInRow; rowIndex++) {
                        if (totalIndex >= group.Items.Count) {
                            break;
                        }

                        DrawItem(palette, group, rowIndex, totalIndex);

                        totalIndex++;
                    }

                    GUI.color = Color.white;
                }
                currentRow++;
            }
        }

        private void DrawItem(Palette palette, PaletteGroup group, int rowIndex, int totalIndex)
        {
            if (!group.Items[totalIndex].HasVariants()) {
                return;
            }

            var drawRectatangle = GetRectForPosition(rowIndex, KalderaEditorUtils.IconButtonSize);

            var paletteItem = group.Items[totalIndex];

            // Make sure the currently selected button is blue
            EditorCustomGUILayout.SetGuiColorState(ScenePlacer.SelectionSettings.SelectedItems.Contains(paletteItem));

            var guiContent = PreviewRenderingUtility.GetGuiContentForItem(paletteItem);
            if (guiContent == null) {
                return;
            }

            if (GUI.Button(drawRectatangle, guiContent, StylesUtility.IconButtonStyle)) {
                if (Event.current.button == 0) {
                    SelectItem(paletteItem);
                } else if(Event.current.button == 1){
                    OpenPrefabInPaletteAsset(palette, group, paletteItem);
                } else if(Event.current.button == 2) {
                    OpenPrefabItem(paletteItem);
                }
            }
        }

        private void DrawVersionFooter()
        {
            EditorGUILayout.LabelField(KalderaEditorUtils.VersionContent);
        }

        private float GetWindowWidth() => position.width - (LeftOffset + RightOffset + ScrollBarWidth);
        private int GetButtonCountInRow(float windowWidth, float iconButtonSize) => Mathf.FloorToInt(windowWidth / iconButtonSize);

        private Rect GetRectForPosition(int rowIndex, float iconButtonSize)
        {
            return new Rect(LeftOffset + (rowIndex * (KalderaEditorUtils.IconButtonSize + ButtonPadding)), 0, iconButtonSize, iconButtonSize);
        }

        private void SelectItem(PaletteItem assetItem)
        {
            if (Event.current.shift) {
                ScenePlacer.SelectionSettings.ToggleSelectedItem(assetItem);
            } else {
                ScenePlacer.SelectionSettings.SetSelectedItem(assetItem);
            }
            NotifyChange();
        }

        private void OpenPrefabItem(PaletteItem assetItem)
        {
            if (!assetItem.HasVariants()) {
                return;
            }

            var variantObject = assetItem.FirstObject();
            EditorGUIUtility.PingObject(variantObject);
            AssetDatabase.OpenAsset(variantObject);
        }

        private void OpenPrefabInPaletteAsset(Palette palette, PaletteGroup group, PaletteItem assetItem)
        {
            EditorGUIUtility.PingObject(palette);
            AssetDatabase.OpenAsset(palette);

            group.IsOpenInEditor = true;
            assetItem.IsOpenInEditor = true;
        }

        private string GetGroupName(PaletteGroup group)
        {
            if (group.GroupName == string.Empty) {
                return "[Nameless group]";
            } else {
                return group.GroupName;
            }
        }

        private void DrawBrushTools()
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                DrawBrushTypeTool();

                var settings = ScenePlacer.SelectionSettings;
                settings.OptionsExtended = EditorGUILayout.Foldout(settings.OptionsExtended, "Tool Options", StylesUtility.BoldFoldoutStyle);

                if (!settings.OptionsExtended) {
                    return;
                }

                ScenePlacer.CurrentBrush.DrawBrushEditor(ScenePlacer);
                ScenePlacer.CurrentBrush.DrawAdditionalSettings(ScenePlacer, settings);
            }

            EditorGUILayout.Space();
        }

        private void DrawBrushTypeTool()
        {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField(KalderaEditorUtils.SelectToolsContent, EditorStyles.boldLabel, GUILayout.Width(KalderaEditorUtils.OptionLabelWidth));
                var currentIndex = Mathf.Min(ScenePlacer.SelectionSettings.SelectedBrushIndex, ScenePlacer.GetBrushMapping().Count - 1);
                EditorGUILayout.LabelField(ScenePlacer.GetBrushMapping()[currentIndex].Name);

            }

            var brushes = ScenePlacer.GetBrushMapping();

            var windowWidth = GetWindowWidth();
            var buttonsInRow = GetButtonCountInRow(windowWidth, KalderaEditorUtils.IconButtonSize);

            var totalIndex = 0;
            var currentRow = 0;

            while (totalIndex < brushes.Count) {

                using (new GUI.GroupScope(GUILayoutUtility.GetRect(windowWidth, KalderaEditorUtils.IconButtonSize))) {
                    for (int rowIndex = 0; rowIndex < buttonsInRow; rowIndex++) {
                        if (totalIndex >= brushes.Count) {
                            break;
                        }
                        var brush = brushes[totalIndex];

                        EditorCustomGUILayout.SetGuiBackgroundColorState(ScenePlacer.SelectionSettings.SelectedBrushIndex == brush.Index);
                        var drawRectatangle = GetRectForPosition(rowIndex, KalderaEditorUtils.IconButtonSize);

                        using (new EditorGUI.DisabledGroupScope(brush.Disabled)) {
                            if (GUI.Button(drawRectatangle, brush.GetButtonContent())) {
                                SetBrushType(brush);
                                NotifyChange();
                            }
                        }

                        totalIndex++;
                    }

                    GUI.color = Color.white;
                }
                currentRow++;
            }

            EditorCustomGUILayout.RestoreGuiColor();
        }

        private void NotifyChange()
        {
            ScenePlacer.NotifyChange();
        }
    }
}