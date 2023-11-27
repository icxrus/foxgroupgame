using System.Collections.Generic;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite
{
    [System.Serializable]
    public class PlacementCollection
    {
        public List<PlacementInformation> Placements = new List<PlacementInformation>();

        public bool HasItems() => Placements.Count > 0;

        public void RotateTowardsPosition(Vector3 position)
        {
            foreach (var placementInformation in Placements) {
                placementInformation.RotateTowardsPosition(position);
            }
        }

        public void RotatePlacement(float rotation)
        {
            var quaternion = Quaternion.Euler(0, rotation, 0);
            foreach (var placement in Placements) {
                placement.Offset = quaternion * placement.UnmodifiedOffset;
                placement.Rotation = quaternion * placement.UnmodifiedRotation;
            }
        }

        public void Hide() {
            foreach(var item in Placements) {
                if(item.GameObject == null) {
                    continue;
                }

                item.GameObject.SetActive(false);
            }
        }

        public void Show() {
            foreach(var item in Placements) {
                if (item.GameObject == null) {
                    continue;
                }

                item.GameObject.SetActive(true);
            }
        }
    }
}