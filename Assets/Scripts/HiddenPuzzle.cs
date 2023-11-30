using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenPuzzle : MonoBehaviour
{
    public bool hasKey = false;
    public BubbleText bubble;
    [SerializeField] private GameObject cagePrefab;

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
            Destroy(cagePrefab);
        }
        else
        {
            //no key yet, tell the player
            Debug.Log("no key bruv");
        }
    }
}
