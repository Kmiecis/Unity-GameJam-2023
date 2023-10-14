using UnityEngine;

namespace Game
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField]
        protected float _scorePerSecond = 1.0f;
        
        [SerializeField]
        protected ScorePickup[] _scoresPrefabs;

        private float _score;
        private ScoreUI _scoreUI;
        private CaveManager _cave;

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
            _scoreUI.UpdateScore(_score);
        }

        private void Awake()
        {
            _scoreUI = FindObjectOfType<ScoreUI>();
            _cave = FindObjectOfType<CaveManager>();
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