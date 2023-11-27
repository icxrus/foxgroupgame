using CollisionBear.WorldEditor.Lite.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite
{
    [CustomEditor(typeof(Palette))]
    public class PaletteEditor : Editor
    {
        private static readonly List<Rect> FieldRects = new List<Rect> {
            new Rect(0, 0, 64, 64),
            new Rect(64, 0, 64, 64),
            new Rect(0, 64, 64, 64),
            new Rect(64, 64, 64, 64)
        };

        private static readonly GUIContent SpacingFactorContent = new GUIContent("Spacing Factor", "Increases or decreases the space between individual objects when placing several at the same time.");
        private static readonly GUIContent MultiplyPrefabScaleContent = new GUIContent("Multiply Prefab Scale", "If set, the scaling is multipled by the scale of the prefab. Otherwise that prefab scale is ignored.");
        private static readonly GUIContent UsePrefabRotationContent = new GUIContent("Use prefab rotation", "If set, the rotation of the prefab is take into consideration when rotating a newly placed object.");
        private static readonly GUIContent UsePrefabHeightContent = new GUIContent("Use Prefab height", "If set, when placing an object, the prefabs set height (position's Y value) will be taken into consideration. If not it is completely ignored.");
        private static readonly GUIContent UseIndividualGroundHeightContent = new GUIContent("Individual ground height", "If set, when placing multiple item, each one will do raycast against the ground to make sure they all have the same relative height to the ground");
        private static readonly GUIContent AllowCollisionContent = new GUIContent("Allow collisions", "If set, when placing objects with colliders, it will not allow placement that causes collisions with other objects (other than the ground).");
        private static readonly GUIContent RotationOffsetContent = new GUIContent("Rotation Offset", "Extra rotation to be applied to the object when placed. Can help with alignment on meshes that are oriented the wrong way.");
        private static readonly GUIContent ItemNamingContent = new GUIContent("Item naming", "If set to item name, the placed game object will take the name of the item. If set to prefab name, it will take the name of the specific prefab used when placing it.");

        private static readonly List<GameObject> DraggedAddedGameObjects = new List<GameObject>();

        private static GUIStyle SpriteGuiStyle = new GUIStyle();

        public override void OnInspectorGUI()
        {
            var category = (Palette)target;

            DrawHeader(category);
            EditorGUILayout.Space();
            using (var changeDetection = new EditorGUI.ChangeCheckScope()) {
                DrawGroups(category);

                if (changeDetection.changed) {
                    EditorUtility.SetDirty(target);
                    Undo.RecordObject(target, $"Updated {nameof(Palette)}");
                }
            }
        }

        protected void DrawHeader(Palette category)
        {
            EditorGUILayout.LabelField("Palette", EditorStyles.boldLabel);

            var shortKeyGuiContent = new GUIContent("Shortcut Key", "Creates a shortcut key command of Shift + <key>");
            category.ShortKey = (KeyCode)EditorGUILayout.EnumPopup(shortKeyGuiContent, category.ShortKey);
        }

        protected void DrawGroups(Palette category)
        {
            EditorGUILayout.LabelField("Palette groups", EditorStyles.boldLabel);

            foreach (var group in category.Groups.ToList()) {
                using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                    using (new EditorGUI.IndentLevelScope(increment: 1)) {
                        DrawGroup(category, group);
                    }
                }
                EditorGUILayout.Space();
            }

            if (GUILayout.Button("Add new Palette group", GUILayout.Height(KalderaEditorUtils.AddButtonHeight))) {
                AddGroup(category);
            }
        }

        protected void DrawGroup(Palette category, PaletteGroup group)
        {
            using (var scope = new EditorGUILayout.HorizontalScope()) {
                group.IsOpenInEditor = EditorGUI.Foldout(new Rect(scope.rect.position, KalderaEditorUtils.FoldoutSize), group.IsOpenInEditor, GUIContent.none, true);
                EditorGUILayout.LabelField("Group:", EditorStyles.boldLabel, GUILayout.Width(KalderaEditorUtils.PrefixLabelWidth));
                if (group.IsOpenInEditor) {
                    group.GroupName = EditorGUILayout.TextField(group.GroupName, GUILayout.Height(KalderaEditorUtils.LineHeight));
                } else {
                    EditorGUILayout.LabelField(group.GroupName, EditorStyles.boldLabel, GUILayout.Height(KalderaEditorUtils.LineHeight));
                }

                if (GUILayout.Button(KalderaEditorUtils.MoveUpIconContent, StylesUtility.TinyButtonStyle)) {
                    category.MoveGroupUp(group);
                }

                if (GUILayout.Button(KalderaEditorUtils.MoveDownIconContent, StylesUtility.TinyButtonStyle)) {
                    category.MoveGroupDown(group);
                }

                if (GUILayout.Button(KalderaEditorUtils.ClearIconContent, StylesUtility.TinyButtonStyle)) {
                    if (group.Items.Count == 0 || EditorUtility.DisplayDialog("Clear group", KalderaEditorUtils.ClearGroupDialog, "Ok", "Cancel")) {
                        ClearGroup(category, group);
                    }
                    GUIUtility.ExitGUI();
                }

                if (GUILayout.Button(KalderaEditorUtils.TrashIconContent, StylesUtility.TinyButtonStyle)) {
                    if (group.Items.Count == 0 || EditorUtility.DisplayDialog("Remove group", KalderaEditorUtils.RemoveGroupDialog, "Ok", "Cancel")) {
                        RemoveGroup(category, group);
                    }
                    GUIUtility.ExitGUI();
                }
            }

            if (group.IsOpenInEditor) {
                EditorGUILayout.Space();
                if (group.Items.Count > 0) {
                    foreach (var item in group.Items.ToList()) {
                        DrawItem(item, category, group);
                    }
                }

                EditorGUILayout.Space();
                using (var scope = new EditorGUILayout.HorizontalScope()) {
                    var controlId = GUIUtility.GetControlID(FocusType.Passive);

                    if (EditorCustomGUILayout.DropTargetButton(new GUIContent("Add Prefab\n[Drag prefab here]"), DraggedAddedGameObjects, GUILayout.Height(48))) {
                        EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, string.Empty, controlId);
                    }
                    if (DraggedAddedGameObjects.Count == 1) {
                        var item = AddItemToGroup(group, DraggedAddedGameObjects.First());
                        item.Name = DraggedAddedGameObjects.First().name;
                    } else if (DraggedAddedGameObjects.Count > 1) {
                        if (EditorUtility.DisplayDialog("Add multiple prefabs", "Do you want to add the game objects as seperate items or variants in a single item?", "Seperate items", "Variants")) {
                            foreach (var gameObject in DraggedAddedGameObjects) {
                                var item = AddItemToGroup(group, gameObject);
                                item.Name = DraggedAddedGameObjects.First().name;
                            }
                        } else {
                            var item = AddItemToGroup(group, DraggedAddedGameObjects.First());
                            item.Name = DraggedAddedGameObjects.First().name;

                            item.GameObjectVariants = new List<GameObject>(DraggedAddedGameObjects);
                        }
                        GUIUtility.ExitGUI();
                    }

                    if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == controlId) {
                        var pickedObject = (GameObject)EditorGUIUtility.GetObjectPickerObject();
                        if (pickedObject != null) {
                            var item = AddItemToGroup(group, pickedObject);
                            item.Name = pickedObject.name;
                            GUI.changed = true;
                        }
                    }

                    if (GUILayout.Button("Add empty Prefab", GUILayout.Height(48))) {
                        AddItemToGroup(group);
                    }
                }
            }
        }

        protected void DrawItem(PaletteItem item, Palette category, PaletteGroup group)
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                using (var scope = new EditorGUILayout.HorizontalScope()) {
                    item.IsOpenInEditor = EditorGUI.Foldout(new Rect(scope.rect.position, KalderaEditorUtils.FoldoutSize), item.IsOpenInEditor, GUIContent.none, true);
                    EditorGUILayout.LabelField("Prefab:", EditorStyles.boldLabel, GUILayout.Width(KalderaEditorUtils.PrefixLabelWidth));
                    if (item.IsOpenInEditor) {
                        item.Name = EditorGUILayout.TextField(item.Name);
                    } else {
                        EditorGUILayout.LabelField(item.Name, EditorStyles.boldLabel);
                    }

                    if (GUILayout.Button(KalderaEditorUtils.MoveUpIconContent, StylesUtility.TinyButtonStyle)) {
                        group.MoveItemUp(item);
                    }

                    if (GUILayout.Button(KalderaEditorUtils.MoveDownIconContent, StylesUtility.TinyButtonStyle)) {
                        group.MoveItemDown(item);
                    }

                    if (GUILayout.Button(KalderaEditorUtils.TrashIconContent, StylesUtility.TinyButtonStyle)) {
                        if (EditorUtility.DisplayDialog("Remove item", "Do you really want to remove this item?", "Ok", "Cancel")) {
                            RemoveItemFromGroup(item, group);
                        }
                        GUIUtility.ExitGUI();
                    }
                }

                if (item.IsOpenInEditor) {
                    DrawToolsItem(category, group, item);
                }
            }
        }

        protected void DrawToolsItem(Palette category, PaletteGroup group, PaletteItem item)
        {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.Space();
                DrawPrefabSelect(item);
                using (new EditorGUILayout.VerticalScope()) {
                    DrawRotationFields(item);

                    EditorGUILayout.Separator();

                    DrawScaleFields(item.Scale);

                    EditorGUILayout.Space();
                    using (var scope = new EditorGUILayout.HorizontalScope()) {
                        item.IsAdvancesOptionsOpenInEditor = EditorGUILayout.Foldout(item.IsAdvancesOptionsOpenInEditor, "Advanced options", true, StylesUtility.BoldFoldoutStyle);
                    }

                    if (item.IsAdvancesOptionsOpenInEditor) {
                        DrawAdvancesOptions(item.AdvancedOptions);
                    }
                }
            }

            EditorGUILayout.Space();
        }

        protected void DrawScaleFields(ScaleInformation scale)
        {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField("Scale", EditorStyles.boldLabel);
                scale.Mode = (ScaleInformation.AxisType)EditorGUILayout.EnumPopup(scale.Mode, EditorStyles.popup, GUILayout.Width(32));
            }

            if (scale.Mode == ScaleInformation.AxisType.SingleAxis) {
                scale.MinScale.x = EditorGUILayout.FloatField(new GUIContent("Scale", "Scale for all axes"), scale.MinScale.x);
            } else if (scale.Mode == ScaleInformation.AxisType.SeperateAxes) {
                scale.MinScale = Vector3Field(new GUIContent("Scale", "Scale for each axis"), scale.MinScale, 80);
            } else if (scale.Mode == ScaleInformation.AxisType.RandomSingleAxis) {
                scale.MinScale.x = EditorGUILayout.FloatField(new GUIContent("Min scale", "Minimum scale for all axes"), scale.MinScale.x);
                scale.MaxScale.x = EditorGUILayout.FloatField(new GUIContent("Max scale", "Maximum scale for all axes"), scale.MaxScale.x);
            } else if (scale.Mode == ScaleInformation.AxisType.RandomSeperateAxes) {
                scale.MinScale = Vector3Field(new GUIContent("Min", "Minimum scale for each axis"), scale.MinScale, 80);
                scale.MaxScale = Vector3Field(new GUIContent("Max", "Maximum scale for each axis"), scale.MaxScale, 80);

                scale.UnitformScaling = EditorGUILayout.Toggle(new GUIContent("Uniform scaling", "If checked, the X, Y and Z axis are all interpolated equally between their min and max values"), scale.UnitformScaling);
            }
        }

        protected Vector3 Vector3Field(GUIContent guiContent, Vector3 value, float labelWidth)
        {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField(guiContent, GUILayout.Width(labelWidth));
                return new Vector3 {
                    x = EditorGUILayout.FloatField(value.x),
                    y = EditorGUILayout.FloatField(value.y),
                    z = EditorGUILayout.FloatField(value.z)
                };
            }
        }

        protected void DrawRotationFields(PaletteItem item)
        {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField("Rotation", EditorStyles.boldLabel);
                item.Rotation.Mode = (RotationInformation.RotationMode)EditorGUILayout.EnumPopup(item.Rotation.Mode, GUILayout.Width(32));
            }

            if (item.Rotation.Mode == RotationInformation.RotationMode.ConstantSeparateAxes) {
                item.Rotation.Constant = Vector3Field(new GUIContent("Constant", ""), item.Rotation.Constant, 80);
            } else if (item.Rotation.Mode == RotationInformation.RotationMode.RandomSeparateAxes) {
                item.Rotation.Min = Vector3Field(new GUIContent("Min", ""), item.Rotation.Min, 80);
                item.Rotation.Max = Vector3Field(new GUIContent("Max", ""), item.Rotation.Max, 80);
            }
        }

        protected float DrawDisableableFloat(float value, bool disabled)
        {
            using (new EditorGUI.DisabledGroupScope(disabled)) {
                if (disabled) {
                    EditorGUILayout.TextField("-");
                    return value;
                } else {
                    return EditorGUILayout.FloatField(value);
                }
            }
        }

        protected void DrawPrefabSelect(PaletteItem item)
        {
            if (item.IsVariantsOpen) {
                DrawVariantOpenPrefabSelect(item);
            } else {
                DrawVariantClosedPrefabSelect(item);
            }
        }

        private void DrawVariantOpenPrefabSelect(PaletteItem item)
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Height(120 + item.GameObjectVariants.Count * KalderaEditorUtils.VaraintSelectionItemHeight))) {
                item.IsVariantsOpen = EditorGUILayout.Foldout(item.IsVariantsOpen, string.Format("Variants ({0})", item.GameObjectVariants.Count), StylesUtility.BoldFoldoutStyle);

                var controlId = GUIUtility.GetControlID(FocusType.Passive);
                if (EditorCustomGUILayout.DropTargetButton(
                    new GUIContent("Add variant\n[Drag prefabs here]"),
                    DraggedAddedGameObjects,
                    GUILayout.Width(KalderaEditorUtils.VariantSelectionBaseWidth),
                    GUILayout.Height(KalderaEditorUtils.AddVariantButtonHeight))
                ) {
                    EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, string.Empty, controlId);
                }
                if (DraggedAddedGameObjects.Count > 0) {
                    foreach (var gameObject in DraggedAddedGameObjects) {
                        item.GameObjectVariants.Add(gameObject);
                    }
                }

                if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == controlId) {
                    var pickedObject = (GameObject)EditorGUIUtility.GetObjectPickerObject();
                    if (pickedObject != null) {
                        item.GameObjectVariants.Add(pickedObject);
                        GUI.changed = true;
                        return;
                    }
                }

                EditorGUILayout.Space();

                using (var scrollScope = new EditorGUILayout.ScrollViewScope(item.VariantsScroll, GUILayout.Width(KalderaEditorUtils.VariantScrollWidth))) {
                    item.VariantsScroll = scrollScope.scrollPosition;
                    for (var i = 0; i < item.GameObjectVariants.Count; i++) {
                        using (new EditorGUILayout.VerticalScope()) {
                            DrawPrefabSelect(item, i);

                            if (GUILayout.Button(KalderaEditorUtils.TrashIconContent, GUILayout.Height(KalderaEditorUtils.AddButtonHeight), GUILayout.Width(KalderaEditorUtils.VariantSelectionBaseWidth))) {
                                if (EditorUtility.DisplayDialog("Remove variant", "Do you really want to remove this variant?", "Ok", "Cancel")) {
                                    item.RemoveVariantAt(i);
                                }
                                GUIUtility.ExitGUI();
                            }
                        }

                        EditorGUILayout.Space();
                    }
                }
            }
        }

        private void DrawVariantClosedPrefabSelect(PaletteItem item)
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(KalderaEditorUtils.VariantScrollWidth))) {
                item.IsVariantsOpen = EditorGUILayout.Foldout(item.IsVariantsOpen, string.Format("Variants ({0})", item.GameObjectVariants.Count), StylesUtility.BoldFoldoutStyle);

                if(item.GameObjectVariants.Count <= 1) {
                    DrawSingleVariant(item);
                } else {
                    DrawMultipleVariants(item);
                }
            }
        }

        private void DrawSingleVariant(PaletteItem item)
        {
            var firstIndex = item.GetFirstIndex();
            DrawPrefabSelect(item, firstIndex);
        }

        private void DrawMultipleVariants(PaletteItem item)
        {
            using (new GUI.GroupScope(GUILayoutUtility.GetRect(128, 128))) {
                var validItems = item.ValidObjects();
                for (int i = 0; i < Mathf.Min(4, validItems.Count); i++) {
                
                    var guiContent = AssetPreview.GetAssetPreview(validItems[i]);
                    if (guiContent == null) {
                        continue;
                    }

                    GUI.DrawTexture(FieldRects[i], guiContent);
                }
            }
        }

        private void DrawPrefabSelect(PaletteItem item, int index)
        {
            using (var check = new EditorGUI.ChangeCheckScope()) {
                var guiContent = PreviewRenderingUtility.GetPreviewTexture(item.GameObjectVariants[index]);
                item.GameObjectVariants[index] = EditorCustomGUILayout.ObjectFieldWithPreview(item.GameObjectVariants[index], guiContent, 128);

                if (check.changed) {
                    if (item.Name == string.Empty && item.GameObjectVariants[index] != null) {
                        item.Name = item.GameObjectVariants[index].name;
                    }
                }
            }
        }

        protected string GetItemName(GameObject asset)
        {
            if (asset == null) {
                return "[Empty Asset]";
            } else {
                return asset.name;
            }
        }

        protected void DrawAdvancesOptions(AdvancedOptions advancedOptions)
        {
            advancedOptions.SpacingFactor = EditorGUILayout.Slider(SpacingFactorContent, advancedOptions.SpacingFactor, 0.1f, 10.0f);
            advancedOptions.MultiplyPrefabScale = EditorGUILayout.Toggle(MultiplyPrefabScaleContent, advancedOptions.MultiplyPrefabScale);
            advancedOptions.UsePrefabRotation = EditorGUILayout.Toggle(UsePrefabRotationContent, advancedOptions.UsePrefabRotation);
            advancedOptions.UsePrefabHeight = EditorGUILayout.Toggle(UsePrefabHeightContent, advancedOptions.UsePrefabHeight);
            advancedOptions.UseIndividualGroundHeight = EditorGUILayout.Toggle(UseIndividualGroundHeightContent, advancedOptions.UseIndividualGroundHeight);
            // advancedOptions.AllowCollision = EditorGUILayout.Toggle(AllowCollisionContent, advancedOptions.AllowCollision);
            advancedOptions.RotationOffset = EditorGUILayout.Vector3Field(RotationOffsetContent, advancedOptions.RotationOffset);
            advancedOptions.NameType = (AdvancedOptions.ItemNameType)EditorGUILayout.EnumPopup(ItemNamingContent, advancedOptions.NameType);
        }

        protected void AddGroup(Palette category)
        {
            category.Groups.Add(new PaletteGroup { GroupName = "", Items = new List<PaletteItem>(), IsOpenInEditor = true, NewGroupItem = new PaletteItem() });
        }

        protected void RemoveGroup(Palette category, PaletteGroup group)
        {
            if (group == null) {
                return;
            }

            if (!category.Groups.Contains(group)) {
                return;
            }

            category.Groups.Remove(group);
        }

        protected void ClearGroup(Palette category, PaletteGroup group)
        {
            if (group == null) {
                return;
            }

            if (!category.Groups.Contains(group)) {
                return;
            };

            group.Items.Clear();
        }

        protected PaletteItem AddItemToGroup(PaletteGroup group)
        {
            var result = new PaletteItem { IsOpenInEditor = true };
            group.Items.Add(result);
            return result;
        }

        protected PaletteItem AddItemToGroup(PaletteGroup group, GameObject gameObject)
        {
            var result = new PaletteItem(gameObject) { IsOpenInEditor = true };
            group.Items.Add(result);
            return result;
        }

        protected void RemoveItemFromGroup(PaletteItem item, PaletteGroup group)
        {
            if (item == null || group == null) {
                return;
            }

            if (!group.Items.Contains(item)) {
                return;
            }

            group.Items.Remove(item);
        }

        protected void ResetAddItemForGroup(PaletteGroup group)
        {
            group.NewGroupItem = new PaletteItem();
        }
    }
}