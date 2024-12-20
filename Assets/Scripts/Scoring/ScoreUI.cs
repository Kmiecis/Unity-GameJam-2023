﻿using TMPro;
using UnityEngine;

namespace Game
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField]
        protected TextMeshProUGUI _text;

        public void UpdateScore(float value)
        {
            _text.text = Mathf.RoundToInt(value).ToString();
        }
    }
}