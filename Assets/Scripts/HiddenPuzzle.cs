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

    public bool hasKey = false;
    public BubbleText bubble;

    private void Start()
    {
        bubble = GetComponent<BubbleText>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && hasKey == true)
        {
            //yes good puzzle solved
            Debug.Log("puzzle solved bruv");
            Invoke("DisableObject", 1f);
        }
        else
        {
            //no key yet, tell the player
            Debug.Log("no key bruv");

            //code for speech bubbles here
        }
    }
    void DisableObject()
    {
        this.gameObject.SetActive(false);
    }
}
