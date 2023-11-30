using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enter2DPuzzle : MonoBehaviour
{
    private Transform tpTarget;
    private CharacterController charControl;
    private bool inPuzzle = false;

    private void Start()
    {
        tpTarget = GameObject.FindGameObjectWithTag("TP2D_In").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected");
        if (other.CompareTag("Player") && !inPuzzle)
        {
            charControl = other.gameObject.GetComponent<CharacterController>();
            charControl.enabled = false;
            other.gameObject.transform.position = tpTarget.position;
            inPuzzle = true;
            charControl.enabled = true;
        }
    }
}
