using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class RayInteraction : MonoBehaviour
{
    [SerializeField] private LayerMask layerToIgnore;
    [SerializeField] private float interactionRange = 12f;
    [SerializeField] private bool isFromMouse = true;

    private System.Func<Ray> rayProvider;

    private PlayerInput input;
    private InputAction interactInput;
    void Start()
    {
        LoadInteractionOption();
        input = GetComponent<PlayerInput>();
        interactInput = input.actions["Interact"];
    }
    void Update()
    {
        RaycastForObject();
    }
    private void OnEnable()
    {
        LoadInteractionOption();
    }
    void LoadInteractionOption()
    {
        //delegate checks which option i chose and then uses that one.
        //whenever the bool is changed in Inspector and the script is re-enabled it updates how it's doing the interaction
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
    void RaycastForObject()
    {
        //shoot a ray from either mouse position or object center (+1 above it) in the direction infront of it based on distance float, ignore specified layers
        Ray ray = rayProvider.Invoke();
        RaycastHit hitData;
        Debug.DrawRay(ray.origin, ray.direction * interactionRange, isFromMouse ? Color.green : Color.red, 0.1f, false);

        if (Physics.Raycast(ray, out hitData, interactionRange, ~layerToIgnore))
            {
            Debug.Log(hitData.collider.gameObject.name);

            //get the inherited interfaces from the hit object
            Component[] interactedObjects = hitData.transform.GetComponents(typeof(IInteractable));
            foreach (IInteractable interactableScripts in interactedObjects)
            {
                if (interactableScripts != null && interactInput.triggered)
                {
                    Debug.Log("E pressed!");
                    //use the inherited method from the object, regardless of which script is using that method
                    interactableScripts.UseInteraction();
                }
            }
        }
    }
}
