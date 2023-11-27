using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite
{
    [System.Serializable]
    public class LayerObjectPlacementMode : IPlacementMode
    {
        public int PlacementLayer;

        public string Name => "Layer Placement";
        public GameObject ParentObject => null;

        public string HintText => "Every collder on the selected layer(s) is chosen. This is a great solution if you've built a level in Unity using a multitude of gameobejct to act as the scene's floor.";

        public RaycastHit? IsValidPlacement(RaycastHit[] raycastHits, int hitCount, PlacementCollection placementCollection)
        {
            if (hitCount == 0) {
                return null;
            }

            var availableObjects = raycastHits
                .Take(hitCount)
                .Where(h => !h.collider.isTrigger)
                .Where(h => h.collider.gameObject.layer == PlacementLayer)
                .OrderBy(h => h.distance)
                .ToList();

            if (availableObjects.Count == 0) {
                return null;
            } else {
                return availableObjects.First();
            }
        }

        public string ValidatePlacementMode()
        {
            return null;
        }

        public void DrawEditor(PaletteWindow paletteWindow)
        {
            PlacementLayer = EditorGUILayout.LayerField("Layer", PlacementLayer, GUILayout.Height(26));
        }

        public bool GameObjectInPlacement(GameObject gameObject) => gameObject.layer == PlacementLayer;
    }
}