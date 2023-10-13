using UnityEngine;

namespace Game
{
    public class World : MonoBehaviour
    {
        public LevelManager levelManager;
        public CaveManager caveManager;
        
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
        }
    }
}