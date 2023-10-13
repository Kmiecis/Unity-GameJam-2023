using UnityEngine;

namespace Game
{
    public class Score : MonoBehaviour
    {
        [SerializeField]
        protected float _value = 1.0f;

        public void OnScored()
        {
            GameObject.FindObjectOfType<ScoreManager>()
                .UpdateScore(_value);
        }
    }
}