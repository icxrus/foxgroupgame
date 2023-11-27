using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PuzzleManager))]
public class CubDataHolder : MonoBehaviour
{
    public GameObject cubPrefab;
    public GameObject deadCubPrefab;
    private PuzzleManager puzzleManager;
    private Transform[] puzzleLocations = new Transform[3];

    //Replace puzzleLocations with this once there is a spawn point for cubs at the puzzle prefabs.
    public Transform[] cubLocationAtPuzzle = new Transform[3];

    //Cub 1 Data
    [Header("Cub 1 Data")]
    public Transform cub1Location;
    public bool isCub1Dead = false;
    private GameObject cub1;

    //Cub 2 Data
    [Header("Cub 2 Data")]
    public Transform cub2Location;
    public bool isCub2Dead = false;
    private GameObject cub2;

    //Cub 3 Data
    [Header("Cub 3 Data")]
    public Transform cub3Location;
    public bool isCub3Dead = false;
    private GameObject cub3;

    private void Start()
    {
        puzzleManager = gameObject.GetComponent<PuzzleManager>();
        puzzleLocations = puzzleManager.ReturnPuzzleLocations();

        InitializeCubsAtPuzzles();
    }

    private void InitializeCubsAtPuzzles()
    {
        cub1Location = puzzleLocations[0];
        cub2Location = puzzleLocations[1];
        cub3Location = puzzleLocations[2];

        cub1 = Instantiate(cubPrefab, cub1Location);
        cub2 = Instantiate(cubPrefab, cub2Location);
        cub3 = Instantiate(cubPrefab, cub3Location);
    }

    /// <summary>
    /// Updates cub life status to dead and removes prefab from puzzle location
    /// </summary>
    /// <param name="cubIndex">0 for cub 1, 1 for cub 2 and 2 for cub 3</param>
    public void CubDeathUpdate(int cubIndex)
    {
        if (cubIndex == 0)
        {
            isCub1Dead = true;
            CubPrefabRemover(cub1);
        }
        else if (cubIndex == 1)
        {
            isCub2Dead = true;
            CubPrefabRemover(cub2);
        }
        else if (cubIndex == 2)
        {
            isCub3Dead = true;
            CubPrefabRemover(cub3);
        }
        else
        {
            Debug.Log("Incorrect index given. Try again. 0 for Cub 1, 1 for Cub 2 and 2 for Cub 3.");
        }
    }

    //Destroys given GameObject. Use with cub prefabs in CubDataHolder!!
    public void CubPrefabRemover(GameObject currentCub)
    {
        Debug.Log("Destroyable cub at position: " + currentCub.transform.position);
        Destroy(currentCub);
        Debug.Log("Cub destroyed.");
    }

    public GameObject ReturnCub1Prefab()
    {
        return cub1;
    }
    public GameObject ReturnCub2Prefab()
    {
        return cub2;
    }
    public GameObject ReturnCub3Prefab()
    {
        return cub3;
    }

    /// <summary>
    /// Spawns a dead cub prefab at a selected Transform location.
    /// </summary>
    /// <param name="location"></param>
    public void SpawnDeadCubPrefab(Transform location)
    {
        Instantiate(deadCubPrefab, location);
    }
}
