using System.IO;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Utils
{
    public static class KalderaEditorUtils
    {

        public const int MiniButtonWidth = 26;
        public const int MiniButtonHeight = 18;

        public const float PrefixLabelWidth = 70;
        public const float TinyButtonWidth = 22;

        public const int ObjectLimitMin = 1;
        public const int ObjectLimitMax = 1000;

        public const int OptionLabelWidth = 100;

        public const int PlacementHintHeight = 70;

        public const float AddButtonHeight = 32;
        public const float IconButtonSize = 48;

        public const float VaraintSelectionItemHeight = 162;
        public const float VariantSelectionBaseWidth = 128;
        public const float VariantScrollWidth = VariantSelectionBaseWidth + 20;
        public const float AddVariantButtonHeight = 48;

        public const string IconsBasePath = "Assets/KalderaPrefabPainter/Images/";

        public const string PluginName = "Kaldera Prefab Painter [Lite]";
        public const string Version = "1.3.4";

        public const string WindowBasePath = "Window/" + PluginName;
        public const string AssetBasePath = "Kaldera Prefab Painter";

        public const string EmptySetTooltip = "Empty editor Palette Set.\nAdd a Palette to this set or select another Palette/Palette set";
        public const string SelectPaletteToolTip = "Please select a Palette or a Palette Set!";
        public const string NoPaletteToolTip = "No Palette / Palette Set\n\nPlease create a Palette or a Palette Set to start using the Prefab Painter!";

        public const string ClearGroupDialog = "Do you really want to clear this group?\\nThis will remove every single item in this group.";
        public const string RemoveGroupDialog = "Do you really want to remove this group?\nThis will remove this group and every single item in it.";

        public static readonly Vector2 FoldoutSize = new Vector2(36, 18);

        public static readonly float LineHeight = EditorGUIUtility.singleLineHeight;

        public static readonly GUIContent TrashIconContent;
        public static readonly GUIContent ClearIconContent;
        public static readonly GUIContent MoveUpIconContent;
        public static readonly GUIContent MoveDownIconContent;
        public static readonly GUIContent ShowInProjectContent;

        public static readonly GUIContent TitleGuiContent;
        public static readonly GUIContent SelectPaletteCollectionContent;
        public static readonly GUIContent SelectPlacementModeContent;
        public static readonly GUIContent SelectPaletteContent;
        public static readonly GUIContent SelectToolsContent;

        public static readonly GUIContent ParentObjectToBaseObjectContent;
        public static readonly GUIContent AllowCollisionContent;
        public static readonly GUIContent OrientToNormalContent;
        public static readonly GUIContent MaintainRotationContent;
        public static readonly GUIContent OrientWithBrushContent;
        public static readonly GUIContent BrushSizeContent;
        public static readonly GUIContent ObjectDensityContent;
        public static readonly GUIContent SprayIntensityContent;
        public static readonly GUIContent BrushDistributionContent;
        public static readonly GUIContent ObjectLimitContent;

        public static readonly GUIContent MoreInformationContent;
        public static readonly GUIContent MousePlaceToolTip;

        public static readonly GUIContent PaletteLabelContent;

        public static readonly GUIContent VersionContent;
        public static readonly GUIContent ReleaseContent;

        public static readonly Mesh PlaneMesh = new Mesh() {
            vertices = new Vector3[] {
                new Vector3(-1, 0, -1),
                new Vector3(1, 0, -1),
                new Vector3(1, 0, 1),
                new Vector3(-1, 0, 1),
            },
            uv = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
            },
            normals = new Vector3[] {
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
            },
            triangles = new int[] {
                2, 1, 0, 0, 3, 2
            }
        };

        public static readonly Material CompassMaterial;
        public static readonly Material ArrowMaterial;
        public static readonly Material LongArrowMaterial;

        static KalderaEditorUtils()
        {
            TrashIconContent = new GUIContent(LoadAssetPath("Icons/TrashIcon.png"), "Remove\nRemoves this item");
            ClearIconContent = new GUIContent(LoadAssetPath("Icons/ClearIcon.png"), "Clear\nClears away all prefabs in this group");
            MoveUpIconContent = new GUIContent(LoadAssetPath("Icons/MoveUpIcon.png"), "Move up");
            MoveDownIconContent = new GUIContent(LoadAssetPath("Icons/MoveDownIcon.png"), "Move down");
            ShowInProjectContent = new GUIContent(LoadAssetPath("Icons/ShowInProjectIcon.png"), "Show asset in project");

            TitleGuiContent = new GUIContent(PluginName, LoadAssetPath("Kaldera_Logo_colored_tiny"));
            SelectPaletteCollectionContent = new GUIContent("Palette Collecion", "Palette Collection\nYou can select either a Palette, or a Palette Set to paint for.");
            SelectPaletteContent = new GUIContent("Palette", "Palette\nYou can select either a Palette, or a Palette Set to paint for.");
            SelectPlacementModeContent = new GUIContent("Placement mode");
            SelectToolsContent = new GUIContent("Tool:", "Determines the shape and properties of the tool.");

            // Options
            AllowCollisionContent = new GUIContent(LoadAssetPath("Icons/AllowCollisionIcon.png"), "Allow collider overlap\nIf set, ignores any collision detection while placing objects");
            ParentObjectToBaseObjectContent = new GUIContent(LoadAssetPath("Icons/ChildObjectsIcon.png"), "Child prefabs to parent collider\nIf set, any placed prefabs will be placed as a child object to the found parent collider");
            OrientToNormalContent = new GUIContent(LoadAssetPath("Icons/OrientToNormalIcon.png"), "Orient to normal\nIf set, placed prefab will be placed perpendicular to the ground where they are placed");
            MaintainRotationContent = new GUIContent(LoadAssetPath("Icons/MaintainRotationIcon.png"), "Maintain rotation\nIf set, the rotation of the next prefabs will not be rotated randomly. Only works for single brush");
            OrientWithBrushContent = new GUIContent(LoadAssetPath("Icons/OrientToPlacementIcon.png"), "Orient with stroke\nIf set randomization of the Y axis will be ignored and placed prefabs will instead be rotated based on the brush's direction.");
            MoreInformationContent = new GUIContent(LoadAssetPath("Icons/MoreInforIcon.png"), "Show more information");
            BrushSizeContent = new GUIContent("Brush Size", "Brush Size\nDetermines the size of the tool.\nShft + [1-5]");
            ObjectDensityContent = new GUIContent("Prefab Density", "Prefab Density\nDetermines the space between the individual prefabs.");
            SprayIntensityContent = new GUIContent("Spray Insensity", "Spray Insensity\nDetermines how many Prefabs are painted per second when spraying");
            BrushDistributionContent = new GUIContent("Distribution", "Distribution\nDetermines how multiple prefabs are spaced out.");
            ObjectLimitContent = new GUIContent("Prefabs limit", "Prefabs limit\nDetermines to max amount of prefabs generated for a single stroke.\n\nNote!\nIncreasing the value too much can cause the editor to be slow and/or unresponsive");

            MousePlaceToolTip = new GUIContent("Press space to release");

            PaletteLabelContent = new GUIContent("Palette");

            VersionContent = new GUIContent($"{PluginName} - Version {Version}");

            var onTopShader = Shader.Find("WorldEditor/OnTopShader");
            CompassMaterial = new Material(onTopShader) {
                mainTexture = LoadAssetPath("Icons/Kaldera_EditorWidget.png")
            };
            ArrowMaterial = new Material(onTopShader) {
                mainTexture = LoadAssetPath("Icons/Kaldera_EditorWidget_arrow.png")
            };
            LongArrowMaterial = new Material(onTopShader) {
                mainTexture = LoadAssetPath("Icons/Kaldera_EditorWidget_arrow_long.png")
            };
        }

        public static Texture2D LoadAssetPath(string localPath) => Resources.Load(Path.GetFileNameWithoutExtension(localPath)) as Texture2D;
    }
}
