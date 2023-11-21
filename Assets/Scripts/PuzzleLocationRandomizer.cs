using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleLocationRandomizer : MonoBehaviour
{
    public List<Transform> possibleLocations = new();
    [Tooltip("0 = 2D, 1 = Hidden, 2 = Parkour")]
    public Transform[] chosenLocations = new Transform[3];

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

        //say which ones were chosen
        Debug.Log($"Chosen locations are: 1. 2D - { chosenLocations[0]}, 2. Hidden - { chosenLocations[1]}, 3. Parkour - { chosenLocations[2]}");
    }

    /// <summary>
    /// Returns the randomized puzzle locations.
    /// </summary>
    /// <returns></returns>
    public Transform ReturnPuzzle2D()
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
    }
}
