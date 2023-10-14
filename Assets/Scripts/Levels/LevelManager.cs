using System;
using Common.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        protected Level[] _levels;

        private CaveManager _caveManager;
        
        private int _current;
        private float _duration;

        public Level Current
            => _levels[_current];

        public void SetInitialLevel()
        {
            SetLevel(0);
        }

        public void SetRandomLevel()
        {
            int index = Random.Range(0, _levels.Length);
            while (index == _current && _levels.Length > 1)
                index = Random.Range(0, _levels.Length);
            SetLevel(index);
        }

        public void SetNextLevel()
        {
            int index = Mathx.NextIndex(_current, _levels.Length);
            SetLevel(index);
        }

        public void SetLevel(int index)
        {
            _current = index;

            _duration = Current.Duration;
            _caveManager.ApplyLevel(Current);
        }

        private void UpdateLevelDuration()
        {
            var dt = Time.deltaTime;

            _duration -= dt;
            if (_duration < 0.0f)
            {
                SetRandomLevel();
            }
        }

        private void Awake()
        {
            _caveManager = FindObjectOfType<CaveManager>();
        }

        private void Update()
        {
            UpdateLevelDuration();
        }
    }
}