using UnityEngine;

namespace Game
{
    public class ScoreManager : MonoBehaviour, IScrollable
    {
        [SerializeField]
        protected float _scorePerDistance = 1.0f;
        
        [SerializeField]
        protected ScorePickup[] _scoresPrefabs;

        private float _score;
        private ScoreUI _scoreUI;
        private CaveManager _cave;
        private GameOverManager _gameOver;

        public void UpdateScore(float value)
        {
            _score += value;
            
            ApplyPointsToUI();
        }
        
        public void ApplyScroll(float dy)
        {
            _score += _scorePerDistance * dy;
            
            ApplyPointsToUI();
        }
        
        private void ApplyPointsToUI()
        {
            if (!_gameOver.IsGameOver)
            {
                _scoreUI.UpdateScore(_score);
            }
        }

        private void Awake()
        {
            _scoreUI = FindObjectOfType<ScoreUI>();
            _cave = FindObjectOfType<CaveManager>();
            _gameOver = FindObjectOfType<GameOverManager>();
        }

        private void Start()
        {
            ApplyPointsToUI();
        }
    }
}