using UnityEngine;

namespace Game
{
    public class World : MonoBehaviour
    {
        public LevelManager levelManager;
        
        public void NextLevel()
        {
            levelManager.SetNextLevel();
        }

        private void Start()
        {
            levelManager.SetInitialLevel();
        }
    }
}