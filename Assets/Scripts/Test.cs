using UnityEngine;

namespace Game
{
    public class Test : MonoBehaviour
    {
        [Header("Damage")]
        public CaveManager caveManager;
        public Vector2Int position;
        public int radius;

        public void ApplyDamage()
        {
            caveManager.Damage(position.x, position.y, radius);
        }
    }
}