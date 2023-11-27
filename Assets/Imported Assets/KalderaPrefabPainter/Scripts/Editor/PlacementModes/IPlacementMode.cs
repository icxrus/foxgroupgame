using UnityEngine;

namespace CollisionBear.WorldEditor.Lite
{
    public interface IPlacementMode
    {
        string Name { get; }

        GameObject ParentObject { get; }
        string ValidatePlacementMode();
        RaycastHit? IsValidPlacement(RaycastHit[] raycastHits, int hitCount, PlacementCollection placementCollection);

        void DrawEditor(PaletteWindow paletteWindow);
        bool GameObjectInPlacement(GameObject gameObject);
        string HintText { get; }
    }
}