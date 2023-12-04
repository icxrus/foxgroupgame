using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PuzzleManager))]
public class CubDataHolder : MonoBehaviour
{
    public GameObject cubPrefab;
    public GameObject deadCubPrefab;
    public GameObject[] cubLocations = new GameObject[3];
    public bool[] isCubDead = new bool[3];
    public int cubsSaved;
    private void Start()
    {
        cubLocations = GameObject.FindGameObjectsWithTag("CubSpawnLoc");
        Invoke("SpawnCubs", 0.25f);
    }
    void SpawnCubs()
    {
        for (int i = 0; i < cubLocations.Length; i++)
        {
            Instantiate(cubPrefab, cubLocations[i].transform);
        }
    }
    /// <summary>
    /// Updates cub life status to dead and removes prefab from puzzle location
    /// </summary>
    /// <param name="index">0 for puzzle 1 cub, 1 for puzzle 2 cub and 2 for puzzle 3 cub</param>
    /// 
    public void CubDeath(int index)
    {
        isCubDead[index] = true;
        Instantiate(deadCubPrefab, cubLocations[index].transform.position, Quaternion.identity);
        Destroy(cubLocations[index]);
    }
    public void CubSaved()
    {
        cubsSaved++;
    }
}
