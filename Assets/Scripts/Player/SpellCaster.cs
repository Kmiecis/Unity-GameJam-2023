﻿using System;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public float attackCooldown = 0.2f;
    private float _timer;
    
    public GameObject projectilePrefab;
    private Camera _camera;

    private bool _isMouseClicked;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        _isMouseClicked = Input.GetMouseButtonDown(0);
    }

    private void FixedUpdate()
    {
        if (_timer > 0)
        {
            _timer -= Time.fixedDeltaTime;
            return;
        }

        if (!_isMouseClicked)
        {
            return;
        }
        
        Cast();
    }

    private void Cast()
    {
        _timer = attackCooldown;
        var currentPosition = transform.position;
        var mousePos = Input.mousePosition;
        mousePos.z = 0;
        var projectile = Instantiate(projectilePrefab, currentPosition, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetData(_camera.ScreenToWorldPoint(mousePos) - currentPosition);
    }
}