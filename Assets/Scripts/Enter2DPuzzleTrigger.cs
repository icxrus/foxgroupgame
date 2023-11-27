using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enter2DPuzzleTrigger : MonoBehaviour
{
    private Transform tpTarget;
    private CharacterController charControl;

    private void Start()
    {
        tpTarget = GameObject.FindGameObjectWithTag("TP2D_In").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected");
        if (other.CompareTag("Player"))
        {
            charControl = other.gameObject.GetComponent<CharacterController>();
            Debug.Log("Attempting to Teleport Player");
            charControl.enabled = false;
            other.gameObject.transform.position = tpTarget.position;
            charControl.enabled = true;
            Debug.Log("Teleported.");
        }
    }
}
