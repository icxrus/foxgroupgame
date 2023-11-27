using CollisionBear.WorldEditor.Lite.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Utils
{
    public static class EditorCustomGUILayout
    {
        private static GUIStyle SpriteGuiStyle = new GUIStyle();
        private static readonly GUIContent FieldNone = new GUIContent("[None]");
        private static readonly Color SelectedColor = new Color(0.5f, 0.8f, 1, 0.5f);

        public static void SetGuiBackgroundColorState(bool state)
        {
            if (state) {
                GUI.backgroundColor = SelectedColor;
            } else {
                GUI.backgroundColor = Color.white;
            }
        }

        public static void SetGuiColorState(bool state)
        {
            if (state) {
                GUI.color = SelectedColor;
            } else {
                GUI.color = Color.white;
            }
        }

        public static void RestoreGuiColor()
        {
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
        }

        public static GameObject ObjectFieldWithPreview(GameObject gameObject, Texture preview, float size = 128)
        {
            var currentEvent = Event.current;
            GUI.BeginGroup(GUILayoutUtility.GetRect(size, size));

            var controllerField = new Rect(0, 0, size, size);
            var fieldRect = new Rect(1, 1, size, size);

            var selectButtonField = new Rect(60, 100, 64, 24);

            if (currentEvent.type == EventType.Repaint) {
                var style = new GUIStyle(GUI.skin.textArea);
                style.Draw(controllerField, GUIContent.none, false, false, false, false);

                if (preview != null) {
                    SpriteGuiStyle.normal.background = (Texture2D)preview;
                } else {
                    SpriteGuiStyle.normal.background = null;
                }

                SpriteGuiStyle.border = new RectOffset(2, 2, 2, 2);
                SpriteGuiStyle.alignment = TextAnchor.MiddleCenter;

                SpriteGuiStyle.Draw(fieldRect, GetObjectPickerGuiContent(gameObject), false, true, false, false);
            }

            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            if (GUI.Button(selectButtonField, "Select")) {
                EditorGUIUtility.ShowObjectPicker<GameObject>(gameObject, false, string.Empty, controlId);
            }

            if (GUI.Button(fieldRect, GUIContent.none, GUIStyle.none)) {
                if (gameObject != null) {
                    EditorGUIUtility.PingObject(gameObject);
                }
            }

            var result = gameObject;

            if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == controlId) {
                result = (GameObject)EditorGUIUtility.GetObjectPickerObject();
                GUI.changed = true;
            }

            if (currentEvent.type.In(EventType.DragPerform, EventType.DragExited, EventType.DragUpdated)) {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (currentEvent.type == EventType.DragPerform) {
                    DragAndDrop.AcceptDrag();

                    result = (GameObject)DragAndDrop.objectReferences.First();
                    GUI.changed = true;
                }
            }

            GUI.EndGroup();
            return result;
        }

        public static bool DropTargetButton(GUIContent content, List<GameObject> droppedItems, params GUILayoutOption[] options)
        {
            var currentEvent = Event.current;
            droppedItems.Clear();

            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            if (GUILayout.Button(content, options)) {
                return true;
            }

            if (currentEvent.type.In(EventType.DragPerform, EventType.DragExited, EventType.DragUpdated)) {
                if (DragAndDrop.objectReferences.Any(g => !(g is GameObject))) {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    return false;
                }

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (currentEvent.type == EventType.DragPerform) {
                    DragAndDrop.AcceptDrag();

                    droppedItems.AddRange(DragAndDrop.objectReferences
                        .Where(g => g is GameObject)
                        .Cast<GameObject>()
                        .ToList()
                    );

                    GUI.changed = true;
                }
            }

            return false;
        }

        private static GUIContent GetObjectPickerGuiContent(GameObject asset)
        {
            if (asset == null) {
                return FieldNone;
            } else {
                return GUIContent.none;
            }
        }
    }
}