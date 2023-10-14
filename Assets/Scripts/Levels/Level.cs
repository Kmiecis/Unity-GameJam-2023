using Common.Mathematics;
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
        [SerializeField]
        protected Range _duration = new Range(4.0f, 6.0f);

        public Material CaveMaterial
            => _caveMaterial;

        public Material CaveBackMaterial
            => _caveBackMaterial;

        public Material BackgroundMaterial
            => _backgroundMaterial;

        public float Duration
            => Random.Range(_duration.min, _duration.max);
    }
}