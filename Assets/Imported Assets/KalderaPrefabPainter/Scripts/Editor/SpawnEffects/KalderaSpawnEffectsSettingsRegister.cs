using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite {
    public static class KalderaSpawnEffectsSettingsRegister {
        private static readonly GUIContent UseSpawnEffectContent = new GUIContent("Use Spawn effect", "Plays a quick spawn effect when placing Prefabs");
        private static readonly GUIContent SpawnDelayContent = new GUIContent("Spawn delay", "Delay between spawned Prefab batches");
        private static readonly GUIContent SpawnGrowTimeContent = new GUIContent("Spawn duration", "Duration Prefabs will take to grow into full scale");
        private static readonly GUIContent SpawnBatchDurationContent = new GUIContent("Max timer", "Max time for a full set to be spawned");
        private static readonly GUIContent PlaySpawnSoundContent = new GUIContent("Play sound", "Play Spawn Sound when a Prefab spawns");

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider() {
            var provider = new SettingsProvider("Collision Bear/", SettingsScope.User) {
                label = "Kaldera World Editor",
                guiHandler = OptionsPageGuiHandler,
                keywords = new HashSet<string>(new[] { "Kaldera", "Prefab Spawner", "World Editor", "Collision Bear" })
            };

            return provider;
        }

        private static void OptionsPageGuiHandler(string search) {
            using (var changeDetection = new EditorGUI.ChangeCheckScope()) {
                KalderaSpawnEffectsSettings.UseSpawnEffects = EditorGUILayout.Toggle(UseSpawnEffectContent, KalderaSpawnEffectsSettings.UseSpawnEffects);

                if (KalderaSpawnEffectsSettings.UseSpawnEffects) {
                    DrawAdvancedOptions();
                }

                if (changeDetection.changed) {
                    KalderaSpawnEffectsSettings.SaveToEditorPrefs();
                }
            }
        }

        private static void DrawAdvancedOptions() {
            EditorGUILayout.Space();
            KalderaSpawnEffectsSettings.SpawnDelay = EditorGUILayout.FloatField(SpawnDelayContent, KalderaSpawnEffectsSettings.SpawnDelay);
            KalderaSpawnEffectsSettings.SpawnGrowTime = EditorGUILayout.FloatField(SpawnGrowTimeContent, KalderaSpawnEffectsSettings.SpawnGrowTime);
            KalderaSpawnEffectsSettings.SpawnBatchDuration = EditorGUILayout.FloatField(SpawnBatchDurationContent, KalderaSpawnEffectsSettings.SpawnBatchDuration);
            KalderaSpawnEffectsSettings.PlaySpawnSound = EditorGUILayout.Toggle(PlaySpawnSoundContent, KalderaSpawnEffectsSettings.PlaySpawnSound);
        }
    }
}
