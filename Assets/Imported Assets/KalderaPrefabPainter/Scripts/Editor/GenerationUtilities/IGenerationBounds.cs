using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Generation
{
    public interface IGenerationBounds
    {
        bool IsWithinBounds(float size, BoxRect box);
    }
}
