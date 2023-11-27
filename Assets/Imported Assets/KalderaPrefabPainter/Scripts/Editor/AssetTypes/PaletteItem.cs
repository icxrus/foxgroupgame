using CollisionBear.WorldEditor.Lite.Extensions;
using CollisionBear.WorldEditor.Lite.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite
{
    [System.Serializable]
    public class RotationInformation
    {
        public enum RotationMode : int
        {
            ConstantSeparateAxes = 0,
            RandomSeparateAxes = 1
        }

        public Vector3 Constant = Vector3.zero;
        public Vector3 Min = Vector3.zero;
        public Vector3 Max = new Vector3(0, 360, 0);

        public RotationMode Mode = RotationMode.RandomSeparateAxes;

        public Vector3 Evaluate()
        {
            if (Mode == RotationMode.RandomSeparateAxes) {
                return new Vector3(Random.Range(Min.x, Max.x), Random.Range(Min.y, Max.y), Random.Range(Min.z, Max.z));
            } else {
                return Constant;
            }
        }
    }

    [System.Serializable]
    public class ScaleInformation
    {
        public enum AxisType : int
        {
            SingleAxis = 0,
            SeperateAxes = 1,
            RandomSingleAxis = 2,
            RandomSeperateAxes = 3
        }

        public AxisType Mode = AxisType.RandomSeperateAxes;

        public Vector3 MinScale = Vector3.one;
        public Vector3 MaxScale = Vector3.one;
        public bool UnitformScaling = true;

        public Vector3 Evaluate()
        {
            if (Mode == AxisType.SingleAxis) {
                return new Vector3(MinScale.x, MinScale.x, MinScale.x);
            } else if (Mode == AxisType.SeperateAxes) {
                return MinScale;
            } else if (Mode == AxisType.RandomSingleAxis) {
                var scaleUnit = Random.Range(0.0f, 1.0f);
                var scale = Mathf.Lerp(MinScale.x, MaxScale.x, scaleUnit);
                return new Vector3(scale, scale, scale);
            } else if (Mode == AxisType.RandomSeperateAxes) {
                var localScale = Vector3.one;

                if (UnitformScaling) {
                    var scaleUnit = Random.Range(0.0f, 1.0f);
                    localScale *= scaleUnit;
                } else {
                    localScale.x = Random.Range(0.0f, 1.0f);
                    localScale.y = Random.Range(0.0f, 1.0f);
                    localScale.z = Random.Range(0.0f, 1.0f);
                }

                var result = new Vector3 {
                    x = Mathf.Lerp(MinScale.x, MaxScale.x, localScale.x),
                    y = Mathf.Lerp(MinScale.y, MaxScale.y, localScale.y),
                    z = Mathf.Lerp(MinScale.z, MaxScale.z, localScale.z)
                };

                return result;
            } else {
                throw new System.NotSupportedException("ScaleAxisType is of unsupported error. Report this to the creator.");
            }
        }
    }

    [System.Serializable]
    public class AdvancedOptions
    {
        public enum ItemNameType : int
        {
            UseItemName = 0,
            UsePrefabName = 1
        }

        public bool AllowPlacementCollisions = false;
        public bool MultiplyPrefabScale = true;
        public bool UsePrefabRotation = false;
        public bool UsePrefabHeight = true;
        public bool UseIndividualGroundHeight = true;
        public bool AllowCollision = false;
        public float SpacingFactor = 1.0f;
        public Vector3 RotationOffset = Vector3.zero;
        public ItemNameType NameType = ItemNameType.UseItemName;
    }

    [System.Serializable]
    public class PaletteItem
    {
        public string Name = string.Empty;
        public List<GameObject> GameObjectVariants;
        public RotationInformation Rotation = new RotationInformation();
        public ScaleInformation Scale = new ScaleInformation();
        public AdvancedOptions AdvancedOptions = new AdvancedOptions();

        // Editor variabels
        public bool IsAdvancesOptionsOpenInEditor = false;
        public bool IsVariantsOpen = false;

        public Vector2 VariantsScroll;

        public bool IsOpenInEditor;

        public PaletteItem()
        {
            GameObjectVariants = new List<GameObject>() { null };       // Initialize it with one empty(null) field
        }

        public PaletteItem(GameObject gameObject)
        {
            GameObjectVariants = new List<GameObject>() { gameObject };       // Initialize it with one empty(null) field
        }

        public int GetFirstIndex() {
            for(int i = 0; i < GameObjectVariants.Count; i ++) {
                if (GameObjectVariants[i] != null) {
                    return i;
                }
            }

            return 0;
        } 

        public GameObject FirstObject()
        {
            foreach(var variant in GameObjectVariants) {
                if(variant != null) {
                    return variant;
                }
            }

            return null;
        }

        public List<GameObject> ValidObjects() => GameObjectVariants.Where(g => g != null).ToList();

        public bool HasVariants() => GameObjectVariants.Any(v => v != null);

        public GameObject GetObjectVariant()
        {
            var validObjects = ValidObjects();
            var index = Random.Range(0, validObjects.Count);
            return validObjects[index];
        }

        public void RemoveVariantAt(int index)
        {
            if (GameObjectVariants.Count == 1) {
                Debug.LogWarning(KalderaEditorUtils.PluginName + ": Can't remove the last variant of an item");
                return;
            }

            GameObjectVariants.RemoveAt(index);
        }

        public Vector3 GetScale()
        {
            return Scale.Evaluate();
        }

        public Vector3 GetRotation(GameObject prefabObject)
        {
            var generatedRotation = Quaternion.Euler(Rotation.Evaluate());

            if (AdvancedOptions.UsePrefabRotation) {
                return (prefabObject.transform.localRotation * generatedRotation).eulerAngles;
            } else {
                return generatedRotation.eulerAngles;
            }
        }

        public float GetItemSize() => GetRendererSize() * AdvancedOptions.SpacingFactor;

        private float GetRendererSize()
        {
            var renderers = FirstObject()
            .GetComponentsInChildren<Renderer>();

            if (renderers.Count() == 0) {
                return 1f;
            } else {
                return renderers.Select(r => r.GetRendererBoundsSize()).Max();
            }
        }
    }
}