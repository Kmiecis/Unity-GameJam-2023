using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public GameObject model;
    public Transform transform;
    public float runSpeed = 40f;
   
    private float _horizontalMove;
    private bool _jump;
    private AnimationController _animationController;

    private void Awake()
    {
        _animationController = model.GetComponent<AnimationController>();
    }

    private void Update()
    {
        _horizontalMove =  Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            _jump = true;
        }
    }
    
    private void FixedUpdate()
    {
        controller.Move(_horizontalMove * Time.fixedDeltaTime, _jump);
        _jump = false;
        if (_horizontalMove == 0)
        {
            _animationController.SetIdleState();
            return;
        }
        
        _animationController.SetWalkState();
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (_horizontalMove > 0 ? -1 : 1), transform.localScale.y);
    }
}
