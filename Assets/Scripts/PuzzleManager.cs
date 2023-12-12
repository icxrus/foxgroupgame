using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Add at least 3+ object transforms to this list to run the randomizer.")]
    [SerializeField] private GameObject cubPrefab;
    public List<Transform> possibleLocations = new();
    [Tooltip("0 = 2D, 1 = Hidden, 2 = Parkour")]
    public Transform[] chosenLocations = new Transform[3];
    public List<GameObject> cubLocationAtPuzzle = new List<GameObject>();
    [SerializeField] private GameObject Puzzle2DPrefab;
    [SerializeField] private GameObject PuzzleHiddenPrefab;
    [SerializeField] private GameObject PuzzleParkourPrefab;

    //hidden puzzle variables
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private List<Transform> leftKeyLocations = new();
    [SerializeField] private List<Transform> rightKeyLocations = new();
    private List<Transform> tmpLoc = new();

    private Transform keyLocation;

    void Awake()
    {
        tmpLoc = possibleLocations;
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

        //Location randomizer at start of game
        for (int i = 0; i < chosenLocations.Length; i++)
        {
            int index = Random.Range(0, possibleLocations.Count);
            chosenLocations[i] = possibleLocations[index];
            possibleLocations.RemoveAt(index); //Make sure there are no duplicate locations
        }
        //say which ones were chosen
        Debug.Log($"Chosen locations are: 1. 2D - { chosenLocations[0].position }, 2. Hidden - { chosenLocations[1].position }, 3. Parkour - { chosenLocations[2].position }");
        InstantiatePuzzles();
    }

    //Instantiates all the puzzles at their correct locations.
    private void InstantiatePuzzles()
    {
        //Instantiate 2D Puzzle
        Instantiate(Puzzle2DPrefab, chosenLocations[0]);

        //instantiate Hidden Puzzle
        Instantiate(PuzzleHiddenPrefab, chosenLocations[1]);
        SetupHidden();

        //instantiate Parkour Puzzle
        Instantiate(PuzzleParkourPrefab, chosenLocations[2]);
        chosenLocations[2].localEulerAngles = new Vector3(0, 0, 0);

        Debug.Log("Spawned puzzles at their randomized locations.");
    }
    void SetupHidden()
    {
        if (chosenLocations[1].tag == "Left")
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

        Instantiate(keyPrefab, keyLocation);

        //say which one was chosen
        Debug.Log($"Key location is: {keyLocation}");
    }
    /// <summary>
    /// Returns the randomized puzzle locations.
    /// </summary>
    /// <returns></returns>
    public Transform[] ReturnPuzzleLocations()
    {
        return chosenLocations;
    }

    private void Update()
    {
        //Editor Specific Debugging
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
        {
            for (int i = 0; i < chosenLocations.Length; i++)
            {
                int index = Random.Range(0, tmpLoc.Count);
                chosenLocations[i] = tmpLoc[index];
                tmpLoc.RemoveAt(index); //Make sure there are no duplicate locations
            }

            InstantiatePuzzles();
        }
#endif
    }
}
