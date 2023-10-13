using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = nameof(Game) + "/" + nameof(Level), fileName = nameof(Level))]
    public class Level : ScriptableObject
    {
        [SerializeField]
        protected Material _caveMaterial;
        [SerializeField]
        protected Material _caveBackMaterial;
        [SerializeField]
        protected Material _backgroundMaterial;

        public Material CaveMaterial
            => _caveMaterial;

        public Material CaveBackMaterial
            => _caveBackMaterial;

        public Material BackgroundMaterial
            => _backgroundMaterial;
    }
}