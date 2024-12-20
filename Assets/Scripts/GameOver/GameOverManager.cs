﻿using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class GameOverManager : MonoBehaviour
    {
        private GameOverUI _ui;
        private PlayerMovement _player;
        private bool _isGameOver;

        public float _yBoundary;

        public bool IsGameOver
            => _isGameOver;
        
        public Sound gameOverSound;
        private SoundsManager _soundsManager;

        public void SetGameOver()
        {
            _isGameOver = true;
            _soundsManager.PlaySound(gameOverSound);
            _ui.gameObject.SetActive(true);
        }

        private void UpdateGameOver()
        {
            if (!_isGameOver && _player.transform.position.y < _yBoundary)
            {
                SetGameOver();
            }
        }

        private void Awake()
        {
            _ui = FindObjectOfType<GameOverUI>();
            _ui.gameObject.SetActive(false);

            _player = FindObjectOfType<PlayerMovement>();
            _soundsManager = FindObjectOfType<SoundsManager>();
        }

        private void Update()
        {
            UpdateGameOver();
        }
    }
}