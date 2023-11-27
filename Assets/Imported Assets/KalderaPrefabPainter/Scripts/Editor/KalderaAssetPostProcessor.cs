using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace CollisionBear.WorldEditor.Lite
{
    public class KalderaAssetPostProcessor : AssetPostprocessor
    {
        private static readonly string AssetFileEnding = ".asset";

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var changedAssets = new List<string>();
            changedAssets.AddRange(importedAssets);
            changedAssets.AddRange(deletedAssets);

            if (changedAssets.Any(a => Path.GetExtension(a.ToLower()) == AssetFileEnding)) {
                PaletteWindow.RefreshAllWindows();
            }
        }
    }
}