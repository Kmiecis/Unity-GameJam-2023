﻿using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ScrollManager : MonoBehaviour
    {
        [SerializeField]
        protected float _speed = 1.0f;
        [SerializeField]
        protected float _acceleration = 1.0f;
        
        [SerializeField]
        protected MonoBehaviour[] _targets;

        private float _offset;
        private List<IScrollable> _offsetteds = new();
        
        public void AddOffsetted(IScrollable scrollable)
        {
            _offsetteds.Add(scrollable);
        }

        private void UpdateSpeed()
        {
            var dt = Time.deltaTime;

            _speed += _acceleration * dt;
        }
        
        private void UpdateScroll()
        {
            var dt = Time.deltaTime;

            ChangeOffset(_speed * dt);
        }
        
        public void ChangeOffset(float dy)
        {
            foreach (var offsetted in _offsetteds)
            {
                offsetted.ApplyScroll(dy);
            }
            
            _offset -= dy;
        }

        private void Awake()
        {
            foreach (var target in _targets)
            {
                if (target is IScrollable offsetted)
                {
                    AddOffsetted(offsetted);
                }
            }
        }
        
        private void Update()
        {
            UpdateSpeed();
            UpdateScroll();
        }
    }
}