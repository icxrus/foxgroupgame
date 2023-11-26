using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Add at least 3+ object transforms to this list to run the randomizer.")]
    public List<Transform> possibleLocations = new();
    [Tooltip("0 = 2D, 1 = Hidden, 2 = Parkour")]
    public Transform[] chosenLocations = new Transform[3];
    [SerializeField] private GameObject cubPrefab;
    [SerializeField] private GameObject Puzzle2DPrefab;
    [SerializeField] private GameObject PuzzleHiddenPrefab;
    [SerializeField] private GameObject PuzzleParkourPrefab;
    private Transform Puzzle2DLocation;
    private Transform PuzzleHiddenLocation;
    private Transform PuzzleParkourLocation;

    //hidden puzzle variables
    [SerializeField] private GameObject keyPrefab;

    [SerializeField] private List<Transform> leftKeyLocations = new();
    [SerializeField] private List<Transform> rightKeyLocations = new();
    private Transform keyLocation;

    //Puzzle status keepers, set to true if puzzle has been failed
    public bool Failed2D = false;
    public bool FailedHidden = false;
    public bool FailedParkour = false;

    void Awake()
    {
        //Location randomizer at start of game
        for (int i = 0; i < chosenLocations.Length; i++)
        {
            int index = Random.Range(0, possibleLocations.Count);
            chosenLocations[i] = possibleLocations[index];
            possibleLocations.RemoveAt(index); //Make sure there are no duplicate locations
        }

        Puzzle2DLocation = chosenLocations[0];
        PuzzleHiddenLocation = chosenLocations[1];
        PuzzleParkourLocation = chosenLocations[2];

        //say which ones were chosen
        Debug.Log($"Chosen locations are: 1. 2D - { Puzzle2DLocation }, 2. Hidden - { PuzzleHiddenLocation }, 3. Parkour - { PuzzleParkourLocation }");

        //make sure puzzles are not failed immediately
        Failed2D = FailedHidden = FailedParkour = false;

        InstantiatePuzzles();
    }

    /// <summary>
    /// Returns the randomized puzzle locations.
    /// </summary>
    /// <returns></returns>
   /* public Transform ReturnPuzzle2D()
    {
        return chosenLocations[0];
    }
    public Transform ReturnPuzzleHidden()
    {
        return chosenLocations[1];
    }
    public Transform ReturnPuzzleParkour()
    {
        return chosenLocations[2];
    }*/

    //Instantiates all the puzzles at their correct locations.
    private bool InstantiatePuzzles()
    {
        //Instantiate 2D Puzzle
        Instantiate(Puzzle2DPrefab, Puzzle2DLocation);

        //instantiate Hidden Puzzle
        Instantiate(PuzzleHiddenPrefab, PuzzleHiddenLocation);
        SetupHidden();

        //instantiate Parkour Puzzle
        Instantiate(PuzzleParkourPrefab, PuzzleParkourLocation);

        //spawn cubs at puzzles
        foreach (Transform puzzleLocs in chosenLocations)
        {
            Instantiate(cubPrefab, puzzleLocs.transform.position, Quaternion.identity);
        }
        Debug.Log("Spawned puzzles at their randomized locations.");
        return true;
    }
    void SetupHidden()
    {
        if (PuzzleHiddenLocation.tag == "Left")
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
}
