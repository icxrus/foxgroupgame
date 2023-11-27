using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CollisionBear.WorldEditor.Lite
{

    public static class SpawnEffects
    {
        private class SpawnedEntity
        {
            public Vector3 TargetScale;
            public GameObject GameObject;
            public float SpawnStart;
            public bool HasSpawned;
            public bool Isfinished;
            public AudioSource AudioSource;
        }

        private const float AudioPitchRange = 0.3f;
        private static AudioClip AudioClip;

        private static Queue<SpawnedEntity> AnimationQueue = new Queue<SpawnedEntity>();
        private static List<SpawnedEntity> ActiveAnimations = new List<SpawnedEntity>();
        private static int SpawnsPerChunk = 1;
        private static bool IsRegistredForUpdate = false;

        private static float LastSpawn = 0;
        private static float VolumeDecay;
        private static float LastUpdate;
        private static List<AudioSource> AudioPool = new List<AudioSource>();

        public static void RegisterObject(GameObject gameObject )
        {
            if (!KalderaSpawnEffectsSettings.UseSpawnEffects) {
                return;
            }

            if (!IsRegistredForUpdate) {
                IsRegistredForUpdate = true;
                EditorApplication.update += Update;
                EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
            }
            
            AnimationQueue.Enqueue(new SpawnedEntity { 
                TargetScale = gameObject.transform.localScale, 
                GameObject = gameObject,
                HasSpawned = false
            });

            gameObject.transform.localScale = Vector3.zero;
        }

        private static void EditorSceneManager_sceneOpened(Scene scene, OpenSceneMode mode)
        {
            if(mode == OpenSceneMode.Single) {
                LastSpawn = Time.realtimeSinceStartup;
            }
        }

        private static void Update()
        {
            var now = Time.realtimeSinceStartup;
            var unspawned = AnimationQueue.Count(o => !o.HasSpawned);

            VolumeDecay = Mathf.Clamp01(VolumeDecay - (now - LastUpdate));

            if(unspawned == 0) {
                SpawnsPerChunk = 1;
            }

            if (LastSpawn + KalderaSpawnEffectsSettings.SpawnDelay <= now && unspawned != 0) {
                LastSpawn = now;
                var maxSpawns = KalderaSpawnEffectsSettings.SpawnBatchDuration / KalderaSpawnEffectsSettings.SpawnDelay;

                if(maxSpawns < unspawned / (float)SpawnsPerChunk)
                    SpawnsPerChunk = Mathf.Max(SpawnsPerChunk, Mathf.CeilToInt(unspawned / (float)maxSpawns));

                var spawnsLeft = SpawnsPerChunk;

                while (spawnsLeft > 0 && AnimationQueue.Count > 0) {
                    var activatedAnimation = AnimationQueue.Dequeue();
                    if (!activatedAnimation.GameObject) {
                        continue;
                    }

                    if (spawnsLeft == SpawnsPerChunk) {
                        var clipVolume = Mathf.Min(1 - Math.Min(VolumeDecay, 0.85f) + Math.Max((4 - unspawned / (float)SpawnsPerChunk) / 3f, 0), 1);
                        activatedAnimation.AudioSource = PlayClip(activatedAnimation.GameObject.transform.position, clipVolume);
                        VolumeDecay += KalderaSpawnEffectsSettings.SpawnDelay * 3;
                    }

                    spawnsLeft--;
                    activatedAnimation.HasSpawned = true;
                    activatedAnimation.SpawnStart = now;
                    ActiveAnimations.Add(activatedAnimation);
                }
            }

            foreach (var spawn in ActiveAnimations) {
                if(spawn.GameObject == null) {
                    continue;
                }

                var timeFactor = Mathf.Min((now - spawn.SpawnStart) / KalderaSpawnEffectsSettings.SpawnGrowTime, 1);
                spawn.GameObject.transform.localScale = spawn.TargetScale * KalderaSpawnEffectsSettings.SpawnAnimation.Evaluate(timeFactor);

                if (timeFactor == 1){
                    spawn.Isfinished = true;
                    if (spawn.AudioSource != null)
                    {
                        spawn.AudioSource.Stop();
                        AudioPool.Add(spawn.AudioSource);
                    }
                }
            }

            ActiveAnimations = ActiveAnimations.Where(o => !o.Isfinished || o.GameObject == null).ToList();
            LastUpdate = now;
        }

        public static AudioSource PlayClip(Vector3 pos, float volume)
        {
            if (!KalderaSpawnEffectsSettings.PlaySpawnSound) {
                return null;
            }

            if(AudioClip == null) {
                AudioClip = Resources.Load<AudioClip>(KalderaSpawnEffectsSettings.SpawnAudioClipPath);
                if (AudioClip == null)
                {
                    KalderaSpawnEffectsSettings.PlaySpawnSound = false;
                    return null;
                }
            }

            AudioSource audioSource;
            if(AudioPool.Count == 0) {
                var go = new GameObject();
                go.hideFlags = HideFlags.HideAndDontSave;
                audioSource = go.AddComponent<AudioSource>();
                audioSource.clip = AudioClip;
                audioSource.spatialBlend = 0;
            } else {
                audioSource = AudioPool[0];
                AudioPool.RemoveAt(0);
            }

            audioSource.transform.position = pos;
            audioSource.volume = volume;
            audioSource.pitch = UnityEngine.Random.Range(1f - AudioPitchRange, 1f + AudioPitchRange);
            audioSource.Play();

            return audioSource;
        }
    }
}
