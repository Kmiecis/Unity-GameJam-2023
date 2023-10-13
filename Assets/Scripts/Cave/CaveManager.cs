using Common.Mathematics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CaveManager : MonoBehaviour
    {
        [SerializeField]
        protected CaveMesh _caveMesh;
        [SerializeField]
        protected CaveCollider _caveCollider;
        [SerializeField]
        protected Transform _ground;

        public CaveGenerator.Input caveInput = CaveGenerator.Input.Default;

        private float[] _noiseMap;
        private bool[] _caveMap;
        private readonly Dictionary<Vector2Int, float> _waits = new ();
        private readonly Dictionary<Vector2Int, float> _offsets = new ();

        public void Damage(int x, int y, int radius, int width, float delay)
        {
            for (int b = -radius; b < +radius; b++)
            {
                for (int a = -radius; a < +radius; a++)
                {
                    if (a * a + b * b < radius * radius)
                    {
                        var key = new Vector2Int { x = a + x, y = b + y };
                        if (key.x > -1 && key.y > -1)
                        {
                            _waits[key] = delay;
                            _offsets[key] = 1.0f;
                        }
                    }
                }
            }
        }
        
        private void ApplyOffsets(float[] noise, int width, float repair)
        {
            var dt = Time.deltaTime;
            var rt = 1.0f / repair;
            
            foreach (var kv in _waits)
            {
                var key = kv.Key;
                var waitValue = kv.Value - dt;
                if (waitValue > 0.0f)
                {
                    _waits[key] = waitValue;
                    var i = Mathx.ToIndex(key.x, key.y, width);
                    noise[i] = 0.0f;
                }
                else
                {
                    var offsetValue = _offsets[key] - rt * dt;
                    if (offsetValue > 0.0f)
                    {
                        _offsets[key] = offsetValue;
                        var i = Mathx.ToIndex(key.x, key.y, width);
                        noise[i] -= offsetValue;
                    }
                    else
                    {
                        _waits.Remove(key);
                        _offsets.Remove(key);
                    }
                }
            }
        }
        
        private void BuildCaveMap()
        {
            CaveGenerator.GenerateNoise(_noiseMap, in caveInput);
            ApplyOffsets(_noiseMap, caveInput.width, caveInput.repair);
            CaveGenerator.GenerateMap(_noiseMap, _caveMap, in caveInput);
        }
        
        private void PositionGround()
        {
            if (_ground != null)
            {
                var width = (caveInput.width - 1);
                var height = (caveInput.height - 1);

                var localPosition = _ground.localPosition;
                var localScale = _ground.localScale;

                localPosition.x = width * 0.5f;
                localPosition.y = height * 0.5f;
                localScale.x = width;
                localScale.y = height;

                _ground.localPosition = localPosition;
                _ground.localScale = localScale;
            }
        }

        private void Build()
        {
            _noiseMap = new float[caveInput.width * caveInput.height];
            _caveMap = new bool[caveInput.width * caveInput.height];
            
            Rebuild();
            PositionGround();
        }

        private void Rebuild()
        {
            BuildCaveMap();
            
            if (_caveMesh != null)
                _caveMesh.SetMap(_caveMap, caveInput.width, caveInput.height);
            if (_caveCollider != null)
                _caveCollider.SetMap(_caveMap, caveInput.width, caveInput.height);
        }
        
#if UNITY_EDITOR
        [Header("Gizmos")]
        [SerializeField]
        protected bool _drawCaveMap;

        private void OnDrawGizmos()
        {
            if (_drawCaveMap)
            {
                var width = caveInput.width;
                var height = caveInput.height;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int i = Mathx.ToIndex(x, y, width);
                        bool isWall = _caveMap[i];
                        Gizmos.color = isWall ? Color.black : Color.white;
                        Gizmos.DrawWireSphere(new Vector3(x, y, 0.0f), 0.2f);
                    }
                }
            }
        }

        private void OnValidate()
        {
            StartCoroutine(BuildNextFrame());
        }

        IEnumerator BuildNextFrame()
        {
            yield return null;
            Build();
        }
#endif
    }
}
