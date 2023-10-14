using System;
using Common;
using Common.Coroutines;
using Common.Extensions;
using Common.Mathematics;
using UnityEngine;

namespace Game
{
    public class CaveMesh : MonoBehaviour
    {
        private readonly int OFFSET_ID = Shader.PropertyToID("_BaseMap");
        private readonly int DISSOLVE_ID = Shader.PropertyToID("_Dissolve");
        private readonly int DISSOLVE_FLIP_ID = Shader.PropertyToID("_DissolveFlip");

        [SerializeField]
        protected CaveMesh _prefab;
        [Header("Components")]
        [SerializeField]
        protected MeshFilter _filter;
        [SerializeField]
        protected MeshRenderer _renderer;
        [SerializeField]
        protected CaveCollider _collider;
        [Header("Input")]
        [SerializeField]
        protected float _wallHeight = 1.0f;
        [SerializeField]
        protected int _renderQueue = 2450;
        [SerializeField]
        protected float _disintegrateDuration = 1.0f;

        private Mesh _mesh;
        private Material _material;

        public void SetMap(bool[] map, int width, int height, bool borderless, float dy)
        {
            _mesh = GetSharedMesh();
            if (_mesh != null && map != null)
            {
                var builder = GenerateMeshBuilder(map, width, height, _wallHeight);
                builder.Overwrite(_mesh);
            }

            if (_collider != null)
            {
                _collider.SetMap(map, width, height, borderless);
            }
            
            var scale = _material.GetTextureScale(OFFSET_ID);
            _material.SetTextureOffset(OFFSET_ID, new Vector2 { y = -Mathf.FloorToInt(dy) * scale.y / height });
        }

        private Mesh GetSharedMesh()
        {
            if (_filter != null)
            {
                var mesh = _filter.sharedMesh;
                if (mesh == null)
                {
                    _filter.sharedMesh = mesh = new Mesh();
                }
                return mesh;
            }
            return null;
        }
        
        public void SetSharedMaterial(Material material)
        {
            UObject.Destroy(ref _material);

            _renderer.sharedMaterial = _material = new Material(material);
            _material.renderQueue = _renderQueue;
        }

        public bool HasSharedMaterial()
        {
            return _material != null;
        }

        public void Disintegrate(Action<CaveMesh> callback)
        {
            _material.SetFloat(DISSOLVE_FLIP_ID, 1.0f);
            _material.SetFloat(DISSOLVE_ID, 0.0f);
            _material.CoFloat(1.0f, DISSOLVE_ID, _disintegrateDuration, Easings.SmoothStep)
                .Then(() => callback(this))
                .Start(this);
        }

        public void Integrate()
        {
            _material.SetFloat(DISSOLVE_FLIP_ID, 0.0f);
            _material.SetFloat(DISSOLVE_ID, 1.0f);
            _material.CoFloat(0.0f, DISSOLVE_ID, _disintegrateDuration, Easings.SmoothStep)
                .Start(this);
        }

        public CaveMesh Copy()
        {
            var copy = Instantiate(_prefab, transform.parent);
            var currentIndex = transform.GetSiblingIndex();
            copy.transform.SetSiblingIndex(currentIndex + 1);
            return copy;
        }

        public void Destroy()
        {
            gameObject.Destroy();
        }

        private static MeshBuilder GenerateMeshBuilder(bool[] map, int width, int height, float wallHeight)
        {
            var builder = new FlatMeshBuilder() { Options = EMeshBuildingOptions.NONE };

            var wallOffset = new Vector3(0.0f, 0.0f, wallHeight);

            var vs = MarchingSquares.Vertices;

            var uvStep = new Vector2(1.0f / width, 1.0f / height);

            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    var v = new Vector3(x, y);

                    var c = MarchingSquares.GetConfiguration(
                        !map[Mathx.ToIndex(x, y, width)],
                        !map[Mathx.ToIndex(x, y + 1, width)],
                        !map[Mathx.ToIndex(x + 1, y + 1, width)],
                        !map[Mathx.ToIndex(x + 1, y, width)]
                    );

                    int i = 0;
                    var ts = MarchingSquares.Triangles[c];
                    for (; i < ts.Length; i += 3)
                    {
                        var t0 = ts[i + 0];
                        var t1 = ts[i + 1];
                        var t2 = ts[i + 2];

                        var v0 = (Vector3)vs[t0] + v - wallOffset;
                        var v1 = (Vector3)vs[t1] + v - wallOffset;
                        var v2 = (Vector3)vs[t2] + v - wallOffset;

                        var uv0 = (vs[t0] + v.XY()) * uvStep;
                        var uv1 = (vs[t1] + v.XY()) * uvStep;
                        var uv2 = (vs[t2] + v.XY()) * uvStep;
                        
                        builder.AddTriangle(v0, v1, v2, uv0, uv1, uv2);
                    }
                    
                    if (c > 0)
                    {
                        var wt0 = ts[i - 1];
                        var wt1 = ts[i - 2];

                        var wv0 = (Vector3)vs[wt0] + v - wallOffset;
                        var wv1 = (Vector3)vs[wt1] + v - wallOffset;
                        var wv2 = wv1 + wallOffset;
                        var wv3 = wv0 + wallOffset;

                        var uv0 = (vs[wt0] + v.XY()) * uvStep;
                        var uv1 = (vs[wt1] + v.XY()) * uvStep;
                        var uv2 = uv0;
                        var uv3 = uv1;

                        builder.AddTriangle(wv0, wv1, wv2, uv0, uv1, uv2);
                        builder.AddTriangle(wv0, wv2, wv3, uv0, uv2, uv3);

                        // Special cases of two walls
                        if (c == 5 || c == 10)
                        {
                            wt0 = ts[1];
                            wt1 = ts[2];

                            wv0 = (Vector3)vs[wt0] + v - wallOffset;
                            wv1 = (Vector3)vs[wt1] + v - wallOffset;
                            wv2 = wv1 + wallOffset;
                            wv3 = wv0 + wallOffset;
                            
                            uv0 = (vs[wt0] + v.XY()) * uvStep;
                            uv1 = (vs[wt1] + v.XY()) * uvStep;
                            uv2 = uv0;
                            uv3 = uv1;

                            builder.AddTriangle(wv2, wv1, wv0, uv2, uv1, uv0);
                            builder.AddTriangle(wv2, wv0, wv3, uv2, uv0, uv3);
                        }
                    }
                }
            }

            return builder;
        }
        
        private void OnDestroy()
        {
            _mesh?.Destroy();
            _material?.Destroy();
        }
    }
}
