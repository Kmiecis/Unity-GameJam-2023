using UnityEngine;

namespace Game
{
    public class World : MonoBehaviour
    {
        public LevelManager levelManager;
        public CaveManager caveManager;
        
        private Level _current;

        private void Start()
        {
            levelManager.SetInitialLevel(caveManager);
        }
    }
}