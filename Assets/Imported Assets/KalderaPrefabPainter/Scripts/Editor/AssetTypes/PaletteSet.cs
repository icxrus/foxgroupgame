using CollisionBear.WorldEditor.Lite.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite
{
    [CreateAssetMenu(menuName = KalderaEditorUtils.AssetBasePath + "/Prefab Palette Collection", fileName = "New Prefab Palette Collection")]
    public class PaletteSet : SelectableAsset
    {
        public List<Palette> Categories = new List<Palette>();
    }
}