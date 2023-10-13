using System;
using Common.Mathematics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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

        private class Damaged
        {
            public float delay;
            public float offset;
        }

        public CaveGenerator.Input caveInput = CaveGenerator.Input.Default;

        private float[] _noiseMap;
        private bool[] _caveMap;
        private readonly Dictionary<Vector2Int, Damaged> _damages = new ();

        [FormerlySerializedAs("damage")] [Header("Test")]
        public Vector2Int position;
        public int radius;

        public void Damage(int x, int y, int radius)
        {
            var rr = radius * radius;
            var rrr = 1.0f / rr;
            
            for (int b = -radius; b < +radius; b++)
            {
                for (int a = -radius; a < +radius; a++)
                {
                    var distance = rr - (a * a + b * b);
                    if (distance > 0.0f)
                    {
                        var key = new Vector2Int { x = a + x, y = b + y };
                        if (key.x > -1 && key.y > -1 &&
                            key.x < caveInput.width && key.y < caveInput.height)
                        {
                            _damages[key] = new Damaged { delay = caveInput.delay, offset = distance * rrr };
                        }
                    }
                }
            }
        }
        
        private void ApplyOffsets(float[] noise, int width, float repair)
        {
            var dt = Time.deltaTime;
            var rt = 1.0f / repair;

            var removed = new List<Vector2Int>();
            foreach (var kv in _damages)
            {
                var key = kv.Key;
                var damage = kv.Value;
                
                damage.delay -= dt;
                if (damage.delay > 0.0f)
                {
                    var i = Mathx.ToIndex(key.x, key.y, width);
                    noise[i] = 0.0f;
                }
                else
                {
                    damage.offset -= rt * dt;
                    if (damage.offset > 0.0f)
                    {
                        var i = Mathx.ToIndex(key.x, key.y, width);
                        noise[i] -= damage.offset;
                    }
                    else
                    {
                        removed.Add(key);
                    }
                }
            }

            foreach (var key in removed)
            {
                _damages.Remove(key);
            }
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
            RebuildNoiseMap();
            RebuildMap();
            ApplyMap();
            PositionGround();
        }

        private void RebuildNoiseMap()
        {
            var size = caveInput.width * caveInput.height;
            if (_noiseMap == null || _noiseMap.Length != size)
                _noiseMap = new float[size];
            
            CaveGenerator.GenerateNoise(_noiseMap, in caveInput);
            ApplyOffsets(_noiseMap, caveInput.width, caveInput.repair);
        }

        private void RebuildMap()
        {
            var size = caveInput.width * caveInput.height;
            if (_caveMap == null || _caveMap.Length != size)
                _caveMap = new bool[size];
            
            CaveGenerator.GenerateMap(_noiseMap, _caveMap, in caveInput);
        }

        private void ApplyMap()
        {
            if (_caveMesh != null)
                _caveMesh.SetMap(_caveMap, caveInput.width, caveInput.height);
            if (_caveCollider != null)
                _caveCollider.SetMap(_caveMap, caveInput.width, caveInput.height, caveInput.borderless);
        }

        private void Update()
        {
            Build();
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
