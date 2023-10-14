using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = nameof(Game) + "/" + nameof(Sound), fileName = nameof(Sound))]
    public class Sound : ScriptableObject
    {
        public AudioClip clip;
        public float volume = 1.0f;
        public bool loop;
    }
}