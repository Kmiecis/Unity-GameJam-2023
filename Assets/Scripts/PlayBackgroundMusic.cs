using UnityEngine;

namespace Game
{
    public class PlayBackgroundMusic : MonoBehaviour
    {
        public Sound music;
        
        private void Start()
        {
            FindObjectOfType<SoundsManager>()
                .PlaySound(music);
        }
    }
}