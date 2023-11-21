using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenPuzzle : MonoBehaviour
{
    // if selected cub is marked as the Hidden puzzle then spawn a barrier to block it fully
    // a trigger is around the cub that checks if player has the key
    // if no key = tell the player what to do; if yes key = remove the barrier
    // check which side of the river the cub is on
    // run randomizer to select where the key spawns; takes into account the river's side and goes to opposite side
    // spawn the key on the randomized key location
    // key is a collision/trigger that will disappear if touched by player
    // if key is touched, mark a Bool on the player that says "Yes, I have Key"
    // when player approaches the barrier with the key, barrier disappears

    public PuzzleLocationRandomizer puzzleManager;
    [SerializeField] private Transform puzzleLocation;

    [SerializeField] private GameObject barrierPrefab;
    [SerializeField] private GameObject keyPrefab;

    [SerializeField] private List<Transform> leftKeyLocations = new();
    [SerializeField] private List<Transform> rightKeyLocations = new();
    [SerializeField] private Transform keyLocation;

    private void Start()
    {
        //puzzle = cub

        puzzleLocation = puzzleManager.ReturnPuzzleHidden();

        Instantiate(barrierPrefab, puzzleLocation);

        if (puzzleLocation.tag == "Left")
        {
            //code to run key randomizer
            int index = Random.Range(0, leftKeyLocations.Count);
            keyLocation = leftKeyLocations[index];
        }
        else if (puzzleLocation.tag == "Right")
        {
            int index = Random.Range(0, rightKeyLocations.Count);
            keyLocation = rightKeyLocations[index];
        }

        Instantiate(keyPrefab, keyLocation);

        //say which ones were chosen
        Debug.Log($"Key location is: {keyLocation}");
    }
}
