using UnityEngine;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        protected Level[] _levels;

        private Level _current;

        public void SetInitialLevel(CaveManager caveManager)
        {
            SetLevel(0, caveManager);
        }

        public void SetLevel(int index, CaveManager caveManager)
        {
            _current = _levels[index];
            caveManager.ApplyLevel(_current);
        }
    }
}