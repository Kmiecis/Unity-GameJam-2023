using System;
using Common.Mathematics;
using System.Collections.Generic;
using Common.Extensions;
using UnityEngine;

namespace Game
{
    public class CaveManager : MonoBehaviour, IScrollable
    {
        [SerializeField]
        protected CaveMesh _caveMesh;
        [SerializeField]
        protected CaveMesh _caveBackMesh;
        [SerializeField]
        protected CaveBackground _caveBackground;

        private class Damaged
        {
            public float delay;
            public float offset;
        }

        public CaveGenerator.Input caveInput = CaveGenerator.Input.Default;
        [Header("Damaging")]
        [Min(0.0f)] public float damageDelay = 1.0f;
        [Min(0.0f)] public float damageRepair = 1.0f;

        private float[] _noiseMap;
        private bool[] _caveMap;
        private bool[] _caveBackMap;
        private readonly Dictionary<Vector2Int, Damaged> _damages = new ();

        private readonly List<CaveMesh> _caveMeshes = new ();
        private readonly List<CaveMesh> _caveBackMeshes = new();

        public void ApplyLevel(Level level)
        {
            void RemoveCaveMesh(CaveMesh caveMesh)
            {
                _caveMeshes.Remove(caveMesh);
                caveMesh.gameObject.Destroy();
            }

            void RemoveCaveBackMesh(CaveMesh caveMesh)
            {
                _caveBackMeshes.Remove(caveMesh);
                caveMesh.gameObject.Destroy();
            }

            CaveMesh SwapCaveMesh(CaveMesh caveMesh, Action<CaveMesh> callback, Material material)
            {
                if (caveMesh.HasSharedMaterial())
                {
                    caveMesh.Disintegrate(callback);
                    caveMesh = caveMesh.Copy();
                }
                caveMesh.SetSharedMaterial(material);
                caveMesh.Integrate();
                return caveMesh;
            }

            _caveMesh = SwapCaveMesh(_caveMesh, RemoveCaveMesh, level.CaveMaterial);
            _caveMeshes.Add(_caveMesh);

            _caveBackMesh = SwapCaveMesh(_caveBackMesh, RemoveCaveBackMesh, level.CaveBackMaterial);
            _caveBackMeshes.Add(_caveBackMesh);

            if (_caveBackground.HasSharedMaterial())
            {
                _caveBackground.Disintegrate();
                _caveBackground = _caveBackground.Copy();
            }
            _caveBackground.SetSharedMaterial(level.BackgroundMaterial);
            _caveBackground.Integrate();
            
            Rebuild();
        }

        public void ApplyScroll(float dy)
        {
            caveInput.dy -= dy;
        }
        
        public void Damage(int x, int y, int radius)
        {
            var position = transform.position;
            x -= Mathf.RoundToInt(position.x);
            y -= Mathf.RoundToInt(position.y);
            
            var rr = radius * radius;
            var rrr = 1.0f / rr;

            var dkey = Mathf.FloorToInt(caveInput.dy);
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
                            key.y -= dkey;
                            _damages[key] = new Damaged { delay = damageDelay, offset = distance * rrr };
                        }
                    }
                }
            }
        }

        private void ApplyDamages(float[] noise, int width, float repair)
        {
            var dt = Time.deltaTime;
            var rt = 1.0f / repair;

            var dkey = Mathf.FloorToInt(caveInput.dy);
            var removed = new List<Vector2Int>();
            foreach (var kv in _damages)
            {
                var key = kv.Key;
                var damage = kv.Value;
                
                var i = Mathx.ToIndex(key.x, key.y + dkey, width);
                
                damage.delay -= dt;
                if (damage.delay > 0.0f)
                {
                    if (i > -1 && i < noise.Length)
                    {
                        noise[i] = 0.0f;
                    }
                    else
                    {
                        removed.Add(key);
                    }
                }
                else
                {
                    damage.offset -= rt * dt;
                    if (damage.offset > 0.0f)
                    {
                        if (i > -1 && i < noise.Length)
                        {
                            noise[i] -= damage.offset;
                        }
                        else
                        {
                            removed.Add(key);
                        }
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

        private void PositionCave()
        {
            var dy = Mathx.Frac(caveInput.dy);

            foreach (var caveMesh in _caveMeshes)
            {
                var position = caveMesh.transform.localPosition;
                position.y = dy;
                caveMesh.transform.localPosition = position;
            }
            foreach (var caveMesh in _caveBackMeshes)
            {
                var position = caveMesh.transform.localPosition;
                position.y = dy;
                caveMesh.transform.localPosition = position;
            }
        }

        private void Rebuild()
        {
            RebuildNoiseMap();
            RebuildMap();
            ApplyMap();
            PositionCave();
        }

        private void RebuildNoiseMap()
        {
            var size = caveInput.width * caveInput.height;
            if (_noiseMap == null || _noiseMap.Length != size)
                _noiseMap = new float[size];

            var noiseCaveInput = caveInput;
            noiseCaveInput.dy = Mathf.Floor(noiseCaveInput.dy);
            noiseCaveInput.dx = Mathf.Floor(noiseCaveInput.dx);
            CaveGenerator.GenerateNoise(_noiseMap, in noiseCaveInput);
        }

        private void RebuildMap()
        {
            var size = caveInput.width * caveInput.height;
            if (_caveMap == null || _caveMap.Length != size)
            {
                _caveMap = new bool[size];
                _caveBackMap = new bool[size];
            }
            
            CaveGenerator.GenerateMap(_noiseMap, _caveBackMap, in caveInput);
            ApplyDamages(_noiseMap, caveInput.width, damageRepair);
            CaveGenerator.GenerateMap(_noiseMap, _caveMap, in caveInput);
        }

        private void ApplyMap()
        {
            foreach (var caveMesh in _caveMeshes)
            {
                caveMesh.SetMap(_caveMap, caveInput.width, caveInput.height, caveInput.borderless, caveInput.dy);
            }
            
            foreach (var caveMesh in _caveBackMeshes)
            {
                caveMesh.SetMap(_caveBackMap, caveInput.width, caveInput.height, caveInput.borderless, caveInput.dy);
            }
        }

        private void Awake()
        {
            caveInput.seed = DateTime.UtcNow.Ticks.ToString();
        }

        private void Update()
        {
            Rebuild();
        }
    }
}
