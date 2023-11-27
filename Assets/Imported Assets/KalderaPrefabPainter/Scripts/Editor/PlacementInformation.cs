using System.Collections.Generic;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite
{
    [System.Serializable]
    public class PlacementInformation
    {
        public PaletteItem Item;
        public GameObject PrefabObject;
        public GameObject GameObject;
        public Vector3 UnmodifiedOffset;
        public Vector3 Offset;
        public Quaternion UnmodifiedRotation;
        public Quaternion Rotation;
        public Vector3 Scale;
        public List<Collider> Colliders = new List<Collider>();

        public PlacementInformation(PaletteItem item, GameObject prefab, Vector3 offset, Vector3 rotationEuler, Vector3 scale)
        {
            Item = item;
            PrefabObject = prefab;
            UnmodifiedOffset = offset;
            Offset = offset;

            Rotation = Quaternion.Euler(rotationEuler);
            UnmodifiedRotation = Rotation;
            Scale = scale;
        }

        public void RotateTowardsPosition(Vector3 position)
        {
            if (GameObject == null) {
                return;
            }

            position.y = GameObject.transform.position.y;
            GameObject.transform.LookAt(position);
            GameObject.transform.rotation *= Quaternion.Euler(Item.AdvancedOptions.RotationOffset);
            Rotation = GameObject.transform.rotation;
        }

        public void SetRotation(Vector3 eulerRotation)
        {
            Rotation = Quaternion.Euler(eulerRotation);

            if (GameObject == null) {
                return;
            }

            GameObject.transform.rotation = Rotation;
        }

        public GameObject CreatePlacementGameObject(Vector3 position, float scaleFactor)
        {
            if (PrefabObject == null) {
                return null;
            }

            var result = GameObject.Instantiate(PrefabObject, Vector3.zero, Rotation);
            result.hideFlags = HideFlags.HideAndDontSave;

            if (Item.AdvancedOptions.UsePrefabHeight) {
                var height = PrefabObject.transform.localPosition.y * scaleFactor;
                Offset.y = height;
            }

            result.transform.position = position + Offset;
            result.transform.localScale = Scale * scaleFactor;

            result.name = PrefabObject.name;
            result.SetActive(false); 

            // Static object occasionally causes horrible performance 
            SetNonStaticRecursiveAndDisableCollider(result);

            GameObject = result;
            return result;
        }

        public void ClearPlacementGameObject()
        {
            if (GameObject == null) {
                return;
            }

            GameObject.DestroyImmediate(GameObject);
        }

        public void ReplacePlacementObject(int newVariantIndex, Vector3 position, float scaleFactor)
        {
            ClearPlacementGameObject();
            var validObjects = Item.ValidObjects();
            PrefabObject = validObjects[newVariantIndex];
            CreatePlacementGameObject(position, scaleFactor);
        }

        private void SetNonStaticRecursiveAndDisableCollider(GameObject gameObject)
        {
            gameObject.isStatic = false;
            foreach (var collider in gameObject.GetComponents<Collider>()) {
                collider.enabled = false;
            }

            foreach (Transform child in gameObject.transform) {
                SetNonStaticRecursiveAndDisableCollider(child.gameObject);
            }
        }
    }
}