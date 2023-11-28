using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenView : MonoBehaviour
{
    public GameObject canvas;
    public GameObject fakeCanvas;

    private void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas2DReal");
        fakeCanvas = GameObject.FindGameObjectWithTag("Canvas2DFake");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameObject.CompareTag("CaveExitA"))
        {
            canvas.SetActive(true);
        }
        else if (other.CompareTag("Player") && gameObject.CompareTag("CaveExitB"))
        {
            fakeCanvas.SetActive(true);
        }
    }
}
