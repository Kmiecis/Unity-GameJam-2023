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
    private Animator _animator;

    private void Awake()
    {
        _animator = model.GetComponent<Animator>();
    }

    private void Update()
    {
        _horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        _animator.SetBool("isMoving", _horizontalMove != 0);

        if (Input.GetButtonDown("Jump"))
        {
            _jump = true;
            _animator.SetBool("isJumping", _jump);
        }
    }

    private void FixedUpdate()
    {
        controller.Move(_horizontalMove * Time.fixedDeltaTime, _jump);
        _jump = false;
        UpdateLocalScale();
    }

    private void UpdateLocalScale()
    {
        if (_horizontalMove != 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (_horizontalMove > 0 ? 1 : -1), transform.localScale.y);
        }
    }

    public void OnLandEvent()
    {
        _animator.SetBool("isJumping", false);
    }
}