using UnityEngine;

namespace Game
{
    public class PlaySound : MonoBehaviour
    {
        public Sound music;
        
        private void Start()
        {
            FindObjectOfType<SoundsManager>()
                .PlaySound(music);
        }
    }
}