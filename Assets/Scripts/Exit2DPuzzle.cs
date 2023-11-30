using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit2DPuzzle : MonoBehaviour
{
    [SerializeField] private Transform teleportTo;
    private CharacterController charControl;
    private void Start()
    {
        charControl = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        teleportTo = GameObject.FindGameObjectWithTag("TP2D_Out").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected");
        if (other.CompareTag("Player"))
        {
            charControl.enabled = false;
            charControl.gameObject.transform.position = teleportTo.position;
            charControl.enabled = true;
        }
    }
}
