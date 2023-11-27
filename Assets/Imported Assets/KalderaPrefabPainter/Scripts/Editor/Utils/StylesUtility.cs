using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Utils {
    public static class StylesUtility
    {
        private static GUIStyle _tinyButtonStyle;
        private static GUIStyle _iconButtonStyle;
        private static GUIStyle _boldFoldoutStyle;
        private static GUIStyle _handleTextStyle;

        public static GUIStyle TinyButtonStyle { 
            get {
                if(_tinyButtonStyle == null) {
                    _tinyButtonStyle = new GUIStyle(EditorStyles.miniButton) {
                        margin = new RectOffset(2, 2, 2, 2),
                        border = new RectOffset(),
                        padding = new RectOffset(1, 1, 1, 1),
                        fixedWidth = KalderaEditorUtils.TinyButtonWidth + 1,
                        fixedHeight = KalderaEditorUtils.TinyButtonWidth + 1
                    };
                }

                return _tinyButtonStyle;
            }
        }

        public static GUIStyle IconButtonStyle { 
            get {
                if(_iconButtonStyle == null) {
                    _iconButtonStyle = new GUIStyle(EditorStyles.miniButton) {
                        margin = new RectOffset(1, 1, 1, 1),
                        border = new RectOffset(),
                        padding = new RectOffset(),
                        fixedWidth = KalderaEditorUtils.IconButtonSize,
                        fixedHeight = KalderaEditorUtils.IconButtonSize
                    };
                }

                return _iconButtonStyle;
            }
        }

        public static GUIStyle BoldFoldoutStyle { 
            get {
                if(_boldFoldoutStyle == null) {
                    _boldFoldoutStyle = new GUIStyle(EditorStyles.foldout) {
                        fontStyle = FontStyle.Bold
                    };
                }

                return _boldFoldoutStyle;
            }
        }

        public static GUIStyle HandleTextStyle { 
            get {
                if(_handleTextStyle == null) {
                    _handleTextStyle = new GUIStyle() {
                        normal = new GUIStyleState {
                            textColor = Color.white,
                        },
                        fontStyle = FontStyle.Bold,
                        fontSize = 14,
                        alignment = TextAnchor.MiddleLeft,
                        imagePosition = ImagePosition.ImageLeft,
                    };
                }

                return _handleTextStyle;
            }
        }
    }
}