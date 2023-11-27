using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enter2DPuzzleTrigger : MonoBehaviour
{
    private Transform tpTarget;

    private void Start()
    {
        tpTarget = GameObject.FindGameObjectWithTag("TP2D_In").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.transform.position = tpTarget.position;
        }
    }
}
