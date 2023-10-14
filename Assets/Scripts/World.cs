using UnityEngine;

namespace Game
{
    public class World : MonoBehaviour
    {
        [Header("Managers")]
        public LevelManager levelManager;
        public CaveManager caveManager;
        public SoundsManager soundsManager;
        [Header("Common")]
        public Sound backgroundMusic;
        
        public void NextLevel()
        {
            levelManager.SetNextLevel(caveManager);
        }

        public void RandomLevel()
        {
            levelManager.SetRandomLevel(caveManager);
        }

        private void Start()
        {
            levelManager.SetInitialLevel(caveManager);
            soundsManager.PlaySound(backgroundMusic);
        }
    }
}