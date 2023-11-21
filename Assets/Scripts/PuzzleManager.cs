using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Add at least 3+ object transforms to this list to run the randomizer.")]
    public List<Transform> possibleLocations = new();
    [Tooltip("0 = 2D, 1 = Hidden, 2 = Parkour")]
    public Transform[] chosenLocations = new Transform[3];
    private Transform Puzzle2DLocation;
    private Transform PuzzleHiddenLocation;
    private Transform PuzzleParkourLocation;
    [SerializeField] private GameObject Puzzle2DPrefab;
    [SerializeField] private GameObject PuzzleHiddenPrefab;
    [SerializeField] private GameObject PuzzleParkourPrefab;

    // Start is called before the first frame update
    void Start()
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

        //instantiate Parkour Puzzle
        Instantiate(PuzzleParkourPrefab, PuzzleParkourLocation);

        Debug.Log("Spawned puzzles at their randomized locations.");
        return true;
    }
}
