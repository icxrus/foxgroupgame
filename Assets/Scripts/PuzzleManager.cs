using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Add all puzzles to this, they key is for HiddenPuzzle.")]
    [SerializeField] private List<GameObject> puzzlePrefabs = new();
    [SerializeField] private GameObject keyPrefab;

    private CubDataHolder cubDataHolder;
    private Transform keyLocation;
    private List<Transform> leftKeyLocations = new();
    private List<Transform> rightKeyLocations = new();
    private List<Transform> possibleLocations = new();


    void Awake()
    {
        cubDataHolder = GetComponent<CubDataHolder>();
        FindLocations();
        SetupLocations();
    }
    void FindLocations()
    {
        //Find all the possible puzzle locations
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("RandomLocation");

        foreach (GameObject item in tmp)
        {
            possibleLocations.Add(item.transform);
        }

        GameObject[] tmp2 = GameObject.FindGameObjectsWithTag("LeftKey");

        foreach (GameObject item in tmp2)
        {
            leftKeyLocations.Add(item.transform);
        }

        GameObject[] tmp3 = GameObject.FindGameObjectsWithTag("RightKey");

        foreach (GameObject item in tmp3)
        {
            rightKeyLocations.Add(item.transform);
        }
    }
    void SetupLocations()
    {
        //instantiate one puzzle prefab per location
        for (int i = 0; i < puzzlePrefabs.Count && i < possibleLocations.Count; i++)
        {
            int randomIndex = Random.Range(0, possibleLocations.Count);
            GameObject puzzlePrefab = puzzlePrefabs[i];
            Instantiate(puzzlePrefab, possibleLocations[randomIndex]);
            cubDataHolder.CreateCubData(puzzlePrefab.tag, possibleLocations[randomIndex]);
            possibleLocations.RemoveAt(randomIndex); //remove used location
        }
        SetupHidden();
    }
    void SetupHidden()
    {
        for (int i = 0; i < cubDataHolder.cubData.Count; i++)
        {
            if (cubDataHolder.cubData[i].tagName == "HiddenPuzzle")
            {
                if (possibleLocations[i].tag == "Left")
                {
                    //if on left, spawn it on the right
                    int index = Random.Range(0, rightKeyLocations.Count);
                    keyLocation = rightKeyLocations[index];
                }
                else
                {
                    //else it must be on the right, spawn it on the left
                    int index = Random.Range(0, leftKeyLocations.Count);
                    keyLocation = leftKeyLocations[index];
                }
                break;
            }
        }
        Instantiate(keyPrefab, keyLocation);
    }
}
