using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInteraction : MonoBehaviour
{
    [SerializeField] private string interactionTag = "Interactable";
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(interactionTag))
        {
            Component[] interactedObjects = other.transform.GetComponents(typeof(IInteractable));
            foreach (IInteractable interactableScripts in interactedObjects)
            {
                if (interactableScripts != null)
                {
                    interactableScripts.EnterInteraction();
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(interactionTag))
        {
            Component[] interactedObjects = other.transform.GetComponents(typeof(IInteractable));
            foreach (IInteractable interactableScripts in interactedObjects)
            {
                if (interactableScripts != null)
                {
                    interactableScripts.ExitInteraction();
                }
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(interactionTag))
        {
            Component[] interactedObjects = other.transform.GetComponents(typeof(IInteractable));
            foreach (IInteractable interactableScripts in interactedObjects)
            {
                if (interactableScripts != null)
                {
                    interactableScripts.StayInteraction();
                }
            }
        }
    }
}
