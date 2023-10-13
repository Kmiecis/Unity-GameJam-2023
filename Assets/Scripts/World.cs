using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IOffsetted
    {
        void ApplyOffset(float dy);
    }
    
    public class World : MonoBehaviour
    {
        public CaveManager caveManager;
        [Header("Scroll")]
        public float speed = 1.0f;

        private float _offset;
        private List<IOffsetted> _offsetteds = new();

        public float Offset
            => _offset;

        public void AddOffsetted(IOffsetted offsetted)
        {
            _offsetteds.Add(offsetted);
        }
        
        public void ChangeOffset(float dy)
        {
            foreach (var offsetted in _offsetteds)
            {
                offsetted.ApplyOffset(dy);
            }
            
            _offset -= dy;
        }

        private void UpdateScroll()
        {
            var dt = Time.deltaTime;

            ChangeOffset(speed * dt);
        }

        private void Start()
        {
            AddOffsetted(caveManager);
        }

        private void Update()
        {
            UpdateScroll();
        }
    }
}