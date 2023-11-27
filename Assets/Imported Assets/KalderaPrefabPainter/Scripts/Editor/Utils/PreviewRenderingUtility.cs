using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Utils
{
    public class PreviewRenderingUtility
    {
        public static Texture2D GetPreviewTexture(Object asset)
        {
            var assetInstanceId = asset.GetInstanceID();
            if (AssetPreview.IsLoadingAssetPreview(assetInstanceId)) {
                return null;
            }

            var result = AssetPreview.GetAssetPreview(asset);
            if(result != null) {
                return result;
            } else {
                return AssetPreview.GetMiniThumbnail(asset);
            }
        }

        private static Texture RenderPreviewForAssetForItem(PaletteItem item) => GetPreviewTexture(item.FirstObject());

        public static GUIContent GetGuiContentForItem(PaletteItem item) {
            var assetPreviewTexture = RenderPreviewForAssetForItem(item);
            if(assetPreviewTexture == null) {
                return null;
            }

            return new GUIContent(assetPreviewTexture, item.Name);
        }
    }
}