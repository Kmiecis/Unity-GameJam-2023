using System;
using UnityEngine;

namespace Game
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField]
        protected float _scorePerSecond = 1.0f;

        private float _score;

        public void UpdateScore(float value)
        {
            _score += value;
            
            ApplyPointsToUI();
        }
        
        private void UpdatePointsPerSecond()
        {
            var dt = Time.deltaTime;
            _score += _scorePerSecond * dt;

            ApplyPointsToUI();
        }

        private void ApplyPointsToUI()
        {
            GameObject.FindObjectOfType<UIScoreHolder>()
                .UpdateScore(_score);
        }

        private void Start()
        {
            ApplyPointsToUI();
        }

        private void Update()
        {
            UpdatePointsPerSecond();
        }
    }
}