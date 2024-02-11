using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PuzzleManager))]
public class CubDataHolder : MonoBehaviour
{
    public GameObject cubPrefab;
    public GameObject deadCubPrefab;
    public GameObject[] cubSpawnLocations = new GameObject[3];
    public bool[] isCubDead = new bool[3];
    public int cubsSaved;

    public List<GameObject> allCubs = new List<GameObject>();
    private void Start()
    {
        cubSpawnLocations = GameObject.FindGameObjectsWithTag("CubSpawnLoc");
        Invoke("SpawnCubs", 0.25f);
        Invoke("GetCubObjects", 0.5f);
    }
    void SpawnCubs()
    {
        for (int i = 0; i < cubSpawnLocations.Length; i++)
        {
            Instantiate(cubPrefab, cubSpawnLocations[i].transform);
        }
    }
    void GetCubObjects()
    {
        GameObject[] taggedCubs = GameObject.FindGameObjectsWithTag("Cub");

        foreach (GameObject cub in taggedCubs)
        {
            allCubs.Add(cub);
        }
    }
    /// <summary>
    /// Updates cub life status to dead and removes prefab from puzzle location
    /// </summary>
    /// <param name="index">0 for puzzle 1 cub, 1 for puzzle 2 cub and 2 for puzzle 3 cub</param>
    /// 
    public void MarkCubDead(int index)
    {
        isCubDead[index] = true;
        Instantiate(deadCubPrefab, cubSpawnLocations[index].transform.position, Quaternion.identity);
        Destroy(cubSpawnLocations[index]);
    }
    public void IncreaseCubsSaved()
    {
        cubsSaved++;
    }
}
