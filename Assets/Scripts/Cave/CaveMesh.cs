using Common;
using Common.Extensions;
using Common.Mathematics;
using UnityEngine;

namespace Custom.CaveGeneration
{
    public class CaveMesh : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        protected MeshFilter _filter;
        [Header("Input")]
        [SerializeField]
        protected float _wallHeight = 1.0f;

        private Mesh _mesh;

        public void SetMap(bool[] map, int width, int height)
        {
            _mesh = GetSharedMesh();
            if (_mesh != null && map != null)
            {
                var builder = GenerateMeshBuilder(map, width, height, _wallHeight);
                builder.Overwrite(_mesh);
            }
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

        private static MeshBuilder GenerateMeshBuilder(bool[] map, int width, int height, float wallHeight)
        {
            var builder = new FlatMeshBuilder() { Options = EMeshBuildingOptions.NONE };

            var wallOffset = new Vector3(0.0f, 0.0f, wallHeight);

            var vs = MarchingSquares.Vertices;

            var uvStep = new Vector2(width, height);

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

                        var uv0 = (vs[t0] + v.XY()) / uvStep;
                        var uv1 = (vs[t1] + v.XY()) / uvStep;
                        var uv2 = (vs[t2] + v.XY()) / uvStep;
                        
                        builder.AddTriangle(v0, v1, v2, uv0, uv1, uv2);
                    }
                }
            }

            return builder;
        }
        
        private void OnDestroy()
        {
            _mesh?.Destroy();
        }
    }
}
