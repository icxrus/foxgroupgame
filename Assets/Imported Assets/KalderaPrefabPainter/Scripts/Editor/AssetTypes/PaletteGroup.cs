using System.Collections.Generic;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite
{
    [System.Serializable]
    public class PaletteGroup
    {
        public string GroupName;
        public List<PaletteItem> Items;
        public PaletteItem NewGroupItem;

        public bool IsOpenInEditor;

        public void MoveItemUp(PaletteItem item)
        {
            var currentIndex = Items.IndexOf(item);
            Items.Remove(item);
            Items.Insert(Mathf.Max(0, currentIndex - 1), item);
        }

        public void MoveItemDown(PaletteItem item)
        {
            var currentIndex = Items.IndexOf(item);
            Items.Remove(item);
            Items.Insert(Mathf.Min(Items.Count, currentIndex + 1), item);
        }
    }
}