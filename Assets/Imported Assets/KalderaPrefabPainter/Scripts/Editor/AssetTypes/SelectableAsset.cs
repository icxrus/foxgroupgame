using UnityEngine;

namespace CollisionBear.WorldEditor.Lite
{
    public abstract class SelectableAsset : ScriptableObject 
    {
        public string GetName() => string.Format("{0} {1} ({2})", name, "\t", GetType().Name);
    }
}