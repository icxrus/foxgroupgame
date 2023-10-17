using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleLocationRandomizer : MonoBehaviour
{
    [SerializeField] private List<Transform> possibleLocations = new();
    [SerializeField] private Transform[] chosenLocations = new Transform[3];

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
    }

    /// <summary>
    /// Returns the randomized puzzle locations.
    /// </summary>
    /// <returns></returns>
    public Transform[] ReturnPuzzleLocations()
    {
        return chosenLocations;
    }
}
