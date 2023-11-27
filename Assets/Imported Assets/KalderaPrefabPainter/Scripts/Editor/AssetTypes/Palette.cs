using CollisionBear.WorldEditor.Lite.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite
{
    [CreateAssetMenu(fileName = "New Prefab Palette", menuName = KalderaEditorUtils.AssetBasePath + "/Prefab Palette")]
    public class Palette : SelectableAsset
    {
        [Tooltip("Used in junction with Shift key")]
        public KeyCode ShortKey;

        public List<PaletteGroup> Groups = new List<PaletteGroup>();

        [System.NonSerialized]
        public Vector2 CurrentScroll;

        public bool HasAnyGroupWithItems()
        {
            foreach (var group in Groups) {
                foreach (var item in group.Items) {
                    foreach (var variant in item.GameObjectVariants) {
                        if (variant != null) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public PaletteGroup GetGroupForItem(PaletteItem item)
        {
            foreach (var group in Groups) {
                if (group.Items.Contains(item)) {
                    return group;
                }
            }

            return null;
        }

        public float GetScrollOffSetForItem(PaletteItem item)
        {
            var result = 0;
            foreach (var group in Groups) {
                foreach (var groupItem in group.Items) {
                    if (groupItem == item) {
                        return result;
                    }
                }
            }

            return result;
        }

        public void MoveGroupUp(PaletteGroup group)
        {
            var currentIndex = Groups.IndexOf(group);
            Groups.Remove(group);
            Groups.Insert(Mathf.Max(0, currentIndex - 1), group);
        }

        public void MoveGroupDown(PaletteGroup group)
        {
            var currentIndex = Groups.IndexOf(group);
            Groups.Remove(group);
            Groups.Insert(Mathf.Min(Groups.Count, currentIndex + 1), group);

        }

        public string GetCategoryName()
        {
            if (ShortKey != KeyCode.None) {
                return string.Format("{0} {1} {2}", GetCategoryBaseName(name), "\t Shft", ShortKey.ToString());
            } else {
                return GetCategoryBaseName(name);
            }
        }

        private string GetCategoryBaseName(string categoryName)
        {
            if (categoryName == string.Empty) {
                return "[Nameless palatte]";
            } else {
                return categoryName;
            }
        }
    }
}