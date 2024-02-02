using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayInteraction : MonoBehaviour
{
    [SerializeField] private RaycastHit hitData;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private float distance = 12f;
    [SerializeField] private bool isFromMouse = true;

    //private Ray ray;
    //private Vector3 pos;

    private System.Func<Ray> rayProvider;

    //yoinked from chatgpt. delegate checks which option i chose and then uses that one.
    void Start()
    {
        if (isFromMouse)
        {
            rayProvider = () => Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            rayProvider = () =>
            {
                Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                return new Ray(pos, transform.forward);
            };
        }
    }
    void Update()
    {
        //shoot a ray from either mouse position or object center (+1 above it) in the direction infront of it based on distance float, only hit the specified layer
        //if (isFromMouse)
        //{
        //    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //}
        //else
        //{
        //    pos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        //    ray = new Ray(pos, transform.forward);
        //}

        //show the ray
        Ray ray = rayProvider.Invoke();
        Debug.DrawRay(ray.origin, ray.direction * distance, isFromMouse ? Color.green : Color.red, 0.1f, false);

        if (Physics.Raycast(ray, out hitData, distance, interactionLayer))
            {
            //if it hit something, run stuff
            Debug.Log(hitData.collider.gameObject.name);

            //get the inherited interfaces from the hit object
            Component[] interactedObjects = hitData.transform.GetComponents(typeof(IInteractable));
            foreach (IInteractable interactedObject in interactedObjects)
            {
                if (interactedObject != null)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Debug.Log("E pressed!");
                        //use the method from the object, regardless of which script is using that method
                        interactedObject.Interact();
                    }
                }
            }
        }
    }
}
