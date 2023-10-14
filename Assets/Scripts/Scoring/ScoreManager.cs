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
    }
}