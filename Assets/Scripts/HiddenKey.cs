using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenKey : MonoBehaviour
{
    private HiddenPuzzle puzzleSource;

    private void Start()
    {
        puzzleSource = GameObject.FindWithTag("HiddenPuzzle").GetComponent<HiddenPuzzle>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("key picked up");
            puzzleSource.hasKey = true;
            Destroy(puzzleSource.bubble);
            this.gameObject.SetActive(false);
            //do some effects or someshit idk
        }
    }
}