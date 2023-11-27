using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite
{
    class KalderaSpawnEffectsSettings : Object
    {
        public const string SpawnAudioClipPath = "sfx/pop";

        private const string KalderaSettingsPath = "KalderaSpawnEffects";

        public static bool UseSpawnEffects = false;
        public static float SpawnGrowTime = .5f;
        public static float SpawnDelay = .08f;
        public static float SpawnBatchDuration = .64f;
        public static bool PlaySpawnSound = true;

        public static readonly AnimationCurve SpawnAnimation = new AnimationCurve()
        {
            keys = new[] {
                new Keyframe(0, 0, 0, 0),
                new Keyframe(0.33f, 1, 2, 2),
                new Keyframe(0.66f, 1, 2, 2),
                new Keyframe(1, 1, 0.5f, 0)
            }
        };


        static KalderaSpawnEffectsSettings() { 
            UseSpawnEffects = EditorPrefs.GetBool(KalderaSettingsPath + "UseSpawnEffects", UseSpawnEffects);
            SpawnGrowTime = EditorPrefs.GetFloat(KalderaSettingsPath + "SpawnGrowTime", SpawnGrowTime);
            SpawnDelay = EditorPrefs.GetFloat(KalderaSettingsPath + "SpawnDelay", SpawnDelay);
            SpawnBatchDuration = EditorPrefs.GetFloat(KalderaSettingsPath + "SpawnBatchDuration", SpawnBatchDuration);
            PlaySpawnSound = EditorPrefs.GetBool(KalderaSettingsPath + "PlaySpawnSound", PlaySpawnSound);
        }

        public static void SaveToEditorPrefs()
        {
            EditorPrefs.SetBool(KalderaSettingsPath + "UseSpawnEffects", UseSpawnEffects);
            EditorPrefs.SetFloat(KalderaSettingsPath + "SpawnGrowTime", SpawnGrowTime);
            EditorPrefs.SetFloat(KalderaSettingsPath + "SpawnDelay", SpawnDelay);
            EditorPrefs.SetFloat(KalderaSettingsPath + "SpawnBatchDuration", SpawnBatchDuration);
            EditorPrefs.SetBool(KalderaSettingsPath + "PlaySpawnSound", PlaySpawnSound);
        }
    }
}
