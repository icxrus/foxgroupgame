using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite
{
    [System.Serializable]
    public class ParentObjectPlacementMode : IPlacementMode
    {
        public Collider ParentCollider;

        public string Name => "Parent Collider";
        public GameObject ParentObject => ParentCollider?.gameObject;
        public string HintText => "A single collider is chosen. This works great if you are painting against a Terrain object or any other single geomery object.";

        public RaycastHit? IsValidPlacement(RaycastHit[] raycastHits, int hitCount, PlacementCollection placementCollection)
        {
            if (hitCount == 0) {
                return null;
            }

            return raycastHits
                .Take(hitCount)
                .Where(h => !h.collider.isTrigger)
                .FirstOrDefault(h => h.collider == ParentCollider);
        }

        public string ValidatePlacementMode()
        {
            if (ParentCollider == null) {
                return "Please select a parent collider!\n\nThe parent collider is where all prefabs will be painted against. It can also be made the parent for all placed prefabs.\nThis object must be existing in the scene";
            }

            return null;
        }

        public void DrawEditor(PaletteWindow paletteWindow)
        {
            var tmpCollider = EditorGUILayout.ObjectField("Collider", ParentCollider, typeof(Collider), allowSceneObjects: true, GUILayout.Height(26)) as Collider;
            if (tmpCollider != ParentCollider) {
                ParentCollider = tmpCollider;

                if (tmpCollider == null) {
                    paletteWindow.ClearSelection();
                }
            }
        }

        public bool GameObjectInPlacement(GameObject gameObject) => gameObject.transform.parent == ParentCollider?.gameObject;
    }
}