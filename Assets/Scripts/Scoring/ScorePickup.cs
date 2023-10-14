using UnityEngine;

namespace Game
{
    public class ScorePickup : MonoBehaviour
    {
        [SerializeField]
        protected float _value = 1.0f;

        public void OnScored()
        {
            FindObjectOfType<ScoreManager>()
                .UpdateScore(_value);
        }
    }
}