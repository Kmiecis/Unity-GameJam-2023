using Common.Mathematics;
using UnityEngine;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        protected Level[] _levels;

        private int _current;

        public Level Current
            => _levels[_current];

        public void SetInitialLevel(CaveManager caveManager)
        {
            SetLevel(0, caveManager);
        }

        public void SetRandomLevel(CaveManager caveManager)
        {
            int index = Random.Range(0, _levels.Length);
            SetLevel(index, caveManager);
        }

        public void SetNextLevel(CaveManager caveManager)
        {
            int index = Mathx.NextIndex(_current, _levels.Length);
            SetLevel(index, caveManager);
        }

        public void SetLevel(int index, CaveManager caveManager)
        {
            _current = index;
            
            caveManager.ApplyLevel(Current);
        }
    }
}