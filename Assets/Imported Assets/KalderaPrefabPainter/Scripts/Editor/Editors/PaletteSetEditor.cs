using CollisionBear.WorldEditor.Lite.Extensions;
using CollisionBear.WorldEditor.Lite.Utils;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite
{
    [CustomEditor(typeof(PaletteSet))]
    public class PaletteSetEditor : Editor
    {
        private ReorderableList ReorderableList;
        private Palette SelectedCategory;

        private int NewCategoryIndex;

        private void OnEnable()
        {
            var categoriesProperties = serializedObject.FindProperty(nameof(PaletteSet.Categories));
            ReorderableList = new ReorderableList(
                serializedObject,
                categoriesProperties,
                draggable: true,
                displayHeader: false,
                displayAddButton: false,
                displayRemoveButton: false
            );

            ReorderableList.headerHeight = 0f;
            ReorderableList.footerHeight = 0f;
            ReorderableList.elementHeightCallback = (_) => EditorGUIUtility.singleLineHeight;
            ReorderableList.drawElementCallback = (rect, i, a, f) => {
                var propertyRect = new Rect(rect.position, new Vector2(rect.width - KalderaEditorUtils.TinyButtonWidth, rect.height));
                var buttonRemoveRect = new Rect(rect.position + new Vector2(rect.width - KalderaEditorUtils.TinyButtonWidth, 0), new Vector2(KalderaEditorUtils.TinyButtonWidth, rect.height));

                if (categoriesProperties.arraySize <= i) {
                    return;
                }

                var arrayProperty = categoriesProperties.GetArrayElementAtIndex(i);
                if (arrayProperty == null) {
                    return;
                }
                EditorGUI.PropertyField(propertyRect, arrayProperty, GUIContent.none);
                if (GUI.Button(buttonRemoveRect, KalderaEditorUtils.TrashIconContent, StylesUtility.TinyButtonStyle)) {
                    RemoveCategory(categoriesProperties, i);
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("Palette Collection", EditorStyles.boldLabel);
            if (ReorderableList.serializedProperty.arraySize == 0) {
                EditorGUILayout.HelpBox("This Collection has no Palettes.", MessageType.Warning);
            } else {
                ReorderableList.DoLayoutList();
            }
            AddPalette(ReorderableList.serializedProperty);
            serializedObject.ApplyModifiedProperties();

            PaletteWindow.RefreshAllWindows();
        }

        private void AddPalette(SerializedProperty categoriesProperty)
        {
            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            if (GUILayout.Button("Add Palette", GUILayout.Height(KalderaEditorUtils.AddButtonHeight))) {
                NewCategoryIndex = categoriesProperty.arraySize;
                categoriesProperty.AddToObjectArray((Palette)null);
                EditorGUIUtility.ShowObjectPicker<Palette>(null, false, string.Empty, controlId);
            }

            if (Event.current.commandName == "ObjectSelectorUpdated") {
                SelectedCategory = (Palette)EditorGUIUtility.GetObjectPickerObject();
                categoriesProperty.GetArrayElementAtIndex(NewCategoryIndex).objectReferenceValue = SelectedCategory;
                categoriesProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        private void RemoveCategory(SerializedProperty serializedProperty, int index)
        {
            Undo.RecordObject(target, $"Removed palette {serializedProperty.GetArrayElementAtIndex(index).name}");
            serializedProperty.RemoveFromObjectArrayAt(index);
            EditorUtility.SetDirty(target);
        }
    }
}