using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using System;

public class CameraZoomController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputProvider;
    [SerializeField] private CinemachineVirtualCamera cmCamera;
    [SerializeField] private float zoomSpeed = 1f, zoomAcceleration = 2.5f, zoomInnerRange = 2f, zoomOuterRange = 5, zoomYAxis = 0f;

    private float currentMiddleRigRadius = 2f, newMiddleRigRadius = 2f;

    public float ZoomYAxis //Sets up the zoom value
    {
        get { return zoomYAxis; }
        set
        {
            if (zoomYAxis == value) return;
            zoomYAxis = value;
            AdjustCameraZoomIndex(ZoomYAxis);
        }
    }

    
    private void Awake()
    {
        //Looking for correct input action and if the scroll wheel has been used or not
        inputProvider.FindActionMap("Player").FindAction("Zoom").performed += cntxt => ZoomYAxis = cntxt.ReadValue<float>();
        inputProvider.FindActionMap("Player").FindAction("Zoom").canceled += cntxt => ZoomYAxis = 0f;
    }

    private void OnEnable() //Required by input system
    {
        inputProvider.FindAction("Zoom").Enable();
    }
    private void OnDisable() //Required by input system
    {
        inputProvider.FindAction("Zoom").Disable();
    }

    private void LateUpdate()
    {
        UpdateZoomLevel();
    }

    private void UpdateZoomLevel() //updates all the rigs to be correct to the zoom level
    {
        if (currentMiddleRigRadius == newMiddleRigRadius) // if we didn't use the scroll wheel, do nothing
            return;

        currentMiddleRigRadius = Mathf.Lerp(currentMiddleRigRadius, newMiddleRigRadius, zoomAcceleration * Time.deltaTime);
        currentMiddleRigRadius = Mathf.Clamp(currentMiddleRigRadius, zoomInnerRange, zoomOuterRange);

        cmCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = currentMiddleRigRadius;
    }

    private void AdjustCameraZoomIndex(float zoomYAxis) //find the correct radius for the camera to zoom to
    {
        if (zoomYAxis == 0) { return; }

        if (zoomYAxis < 0)
        {
            newMiddleRigRadius = currentMiddleRigRadius + zoomSpeed;
        }

        if (zoomYAxis > 0)
        {
            newMiddleRigRadius = currentMiddleRigRadius - zoomSpeed;
        }

    }


}
//Tutorial by Cookie Software Design - https://www.youtube.com/watch?v=bVoJ3-BMNi0&t

