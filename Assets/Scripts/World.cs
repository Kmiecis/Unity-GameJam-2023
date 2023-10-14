using UnityEngine;

namespace Game
{
    public class World : MonoBehaviour
    {
        [Header("Managers")]
        public LevelManager levelManager;
        
        public void NextLevel()
        {
            levelManager.SetNextLevel();
        }

        public void RandomLevel()
        {
            levelManager.SetRandomLevel();
        }

        private void Start()
        {
            levelManager.SetInitialLevel();
        }
    }
}