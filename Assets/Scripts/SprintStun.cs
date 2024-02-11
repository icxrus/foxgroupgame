using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class SprintStun : MonoBehaviour
{
    [SerializeField] private float slowedRotSpeed = 1.0f;
    [SerializeField] private int stunTimer = 2;
    [SerializeField] private float timerDelay = 1.0f;
    [SerializeField] private LayerMask layerMasks;

    private ThirdPersonController _controller;
    private CharacterController _charCont;
    private StarterAssetsInputs _input;
    private float _initRotSpeed;
    private float _initMoveSpeed;
    private float _initSprintSpeed;
    private bool _hasCollided = false;

    public void OnSprint(InputValue value)
    {
        if (value.isPressed)
        {
            _controller.RotationSmoothTime = slowedRotSpeed;
        }
        else
        {
            _controller.RotationSmoothTime = _initRotSpeed;
        }
    }

    private void Start()
    {
        _charCont = GetComponent<CharacterController>();
        _input = GetComponent<StarterAssetsInputs>();
        _controller = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
        _initRotSpeed = _controller.RotationSmoothTime;
        _initMoveSpeed = _controller.MoveSpeed;
        _initSprintSpeed = _controller.SprintSpeed;

    }

    private void OnControllerColliderHit(ControllerColliderHit collision)
    {
        float currentHorizontalSpeed = new Vector3(_charCont.velocity.x, 0.0f, _charCont.velocity.z).magnitude;
        float sprintSpeed = _controller.SprintSpeed - 0.1f;

        //Debug.Log("collision happened");
        if (_input.sprint == true && layerMasks.Includes(collision.gameObject.layer) && _hasCollided == false && currentHorizontalSpeed >= sprintSpeed)
        {
            _hasCollided = true;
            StartCoroutine(GetStunned());
        } 
    }

    private IEnumerator GetStunned()
    {
       // Debug.Log("START. this should run ONCE");
        _controller.MoveSpeed = 0f;
        _controller.SprintSpeed = 0f;

        for (int i = 0; i < stunTimer; i++)
        {
            //stuff
           // Debug.Log("STUFF. this should run LOTS");

            yield return new WaitForSeconds(timerDelay);
        }
       // Debug.Log("END. this should run ONCE");
        _hasCollided = false;
        _controller.MoveSpeed = _initMoveSpeed;
        _controller.SprintSpeed = _initSprintSpeed;
    }
}

//yoinked this from a tutorial (kinda)
public static class LayerMaskExtensions
{
    public static bool Includes(
    this LayerMask mask,
    int layer)
    {
        return (mask.value & 1 << layer) > 0;
    }
}
