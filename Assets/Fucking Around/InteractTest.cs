using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTest : MonoBehaviour, IInteractable, IEventRunner
{
    public void Interact()
    {

        Debug.Log("I've been interacted with!");
        gameObject.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

    }
    public void RunEvent()
    {
            Debug.Log("Running event!");
    }
}
