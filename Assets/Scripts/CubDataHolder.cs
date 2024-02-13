using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(PuzzleManager))]
public class CubDataHolder : MonoBehaviour
{
    public GameObject cubPrefab;
    public GameObject deadCubPrefab;
    public List<GameObject> cubSpawnLocations = new List<GameObject>();
    public int cubsSaved;

    public List<CubData> cubData = new List<CubData>();
    public class CubData
    {
        public string tagName;
        public Transform puzzleSpawn;
        public Transform cubAtPuzzle;
        public bool isCubDead;

        public CubData(string puzzleTag, Transform puzzleSpawnLocation)
        {
            tagName = puzzleTag;
            puzzleSpawn = puzzleSpawnLocation;
            cubAtPuzzle = null;
            isCubDead = false;
        }
    }
    public void CreateCubData(string tagName, Transform spawnLocation)
    {
        CubData pD = new CubData(tagName, spawnLocation);
        cubData.Add(pD);
    }
    public void RemoveCubData(string tagName)
    {
        for (int i = 0; i < cubData.Count; i++)
        {
            if (cubData[i].tagName == tagName)
            {
                cubData.RemoveAt(i);
                break;
            }
        }
    }
    private void Start()
    {
        cubSpawnLocations = GameObject.FindGameObjectsWithTag("CubSpawnLoc").ToList();
        Invoke("SpawnCubs", 0.25f);
    }
    void SpawnCubs()
    {
        for (int i = 0; i < cubSpawnLocations.Count; i++)
        {
            Instantiate(cubPrefab, cubSpawnLocations[i].transform);
        }
    }
    public void MarkCubDead(CubData puzzleData)
    {
        puzzleData.isCubDead = true;
        Instantiate(deadCubPrefab, puzzleData.cubAtPuzzle.transform.position, Quaternion.identity);
        Destroy(puzzleData.cubAtPuzzle.gameObject);
    }
    public void IncreaseCubsSaved()
    {
        cubsSaved++;
    }
}
