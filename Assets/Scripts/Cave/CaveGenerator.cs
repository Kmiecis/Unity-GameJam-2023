using Common.Mathematics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class CaveGenerator
    {
        public const bool kRoom = true;
        public const bool kWall = false;
        public const float kRoomValue = 0.0f;
        public const float kWallValue = 1.0f;

        private struct Line
        {
            public Vector2Int a;
            public Vector2Int b;
        }

        [Serializable]
        public struct Input
        {
            [Header("Map generation")]
            public int width;
            public int height;
            public int smooths;
            [Range(0.0f, 1.0f)]
            public float fill;
            public string seed;

            [Header("Noise")]
            public int octaves;
            [Range(0.0f, 1.0f)]
            public float persistence;
            [Range(1.0f, 5.0f)]
            public float lacunarity;
            public float dx;
            public float dy;
            [Min(0.1f)]
            public float sx;
            [Min(0.1f)]
            public float sy;

            [Header("Map processing")]
            public int wallSizeThreshold;
            public int roomSizeThreshold;
            public int passageWidth;
            public int borderHeight;
            public int borderWidth;
            public bool borderless;

            public static readonly Input Default = new Input
            {
                width = 64,
                height = 64,
                smooths = 4,
                fill = 0.5f,
                seed = "",
                octaves = 1,
                persistence = 0.5f,
                lacunarity = 2.0f,
                sx = 1.0f,
                sy = 1.0f,
                wallSizeThreshold = 5,
                roomSizeThreshold = 5,
                passageWidth = 2,
                borderHeight = 2,
                borderWidth = 2,
                borderless = false,
            };
        }

        public static void GenerateNoise(float[] noise, in Input input)
        {
            Noisex.GetNoiseMap(noise, input.width, input.height, input.octaves, input.persistence, input.lacunarity, input.dx, input.dy, input.sx, input.sy, input.seed.GetHashCode());
            ApplyBorder(noise, input.width, input.height, input.borderHeight, input.borderWidth, input.borderless);
        }
        
        public static void GenerateMap(float[] noise, bool[] map, in Input input)
        {
            ConvertNoiseToMap(noise, map, input.width, input.height, input.fill);
    
            /*
            var roomRegions = GetRegionsByType(map, input.width, input.height, kRoom);
            var removedRoomRegions = RemoveRegionsUnderThreshold(roomRegions, input.roomSizeThreshold);
            FlipRegions(removedRoomRegions, map, input.width);

            var wallRegions = GetRegionsByType(map, input.width, input.height, kWall);
            var removedWallRegions = RemoveRegionsUnderThreshold(wallRegions, input.wallSizeThreshold);
            FlipRegions(removedWallRegions, map, input.width);

            var rooms = CreateRooms(roomRegions, map, input.width, input.height);
            var passages = FindPassages(rooms);
            ClearPassages(passages, map, input.width, input.height, input.passageWidth, input.borderWidth, input.borderHeight);
            */
        }

        private static void ConvertNoiseToMap(float[] noise, bool[] map, int width, int height, float fill)
        {
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int i = Mathx.ToIndex(x, y, width);
                    map[i] = noise[i] < (1.0f - fill);
                }
            }
        }

        private static void ApplyBorder(float[] map, int width, int height, int borderWidth, int borderHeight, bool borderless)
        {
            var bheight = Math.Abs(borderHeight);
            var bheightType = borderHeight > 0 ? kWallValue : kRoomValue;
            if (bheight != 0)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < bheight; x++)
                    {
                        int i = Mathx.ToIndex(x, y, width);
                        map[i] = bheightType;
                    }

                    for (int x = width - bheight; x < width; x++)
                    {
                        int i = Mathx.ToIndex(x, y, width);
                        map[i] = bheightType;
                    }
                }
            }

            var bwidth = Math.Abs(borderWidth);
            var bwidthType = borderWidth > 0 ? kWallValue : kRoomValue;
            if (bwidth != 0)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < bwidth; y++)
                    {
                        int i = Mathx.ToIndex(x, y, width);
                        map[i] = bwidthType;
                    }

                    for (int y = height - bwidth; y < height; y++)
                    {
                        int i = Mathx.ToIndex(x, y, width);
                        map[i] = bwidthType;
                    }
                }
            }

            if (borderless)
            {
                for (int y = 0; y < height; y++)
                {
                    int i = Mathx.ToIndex(0, y, width);
                    map[i] = kRoomValue;

                    int j = Mathx.ToIndex(width - 1, y, width);
                    map[j] = kRoomValue;
                }

                for (int x = 0; x < width; x++)
                {
                    int i = Mathx.ToIndex(x, 0, width);
                    map[i] = kRoomValue;

                    int j = Mathx.ToIndex(x, height - 1, width);
                    map[j] = kRoomValue;
                }
            }
        }

        private static List<List<Vector2Int>> GetRegionsByType(bool[] map, int width, int height, bool type)
        {
            var result = new List<List<Vector2Int>>();

            var isChecked = new bool[width * height];
            var mapRange = new Range2Int(0, 0, width - 1, height - 1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = Mathx.ToIndex(x, y, width);
                    if (map[i] == type && !isChecked[i])
                    {
                        var region = new List<Vector2Int>();
                        var toCheck = new Queue<Vector2Int>();

                        toCheck.Enqueue(new Vector2Int(x, y));
                        isChecked[i] = true;

                        while (toCheck.Count > 0)
                        {
                            var current = toCheck.Dequeue();
                            region.Add(current);

                            foreach (var direction in Axes.All2D)
                            {
                                var neighbour = current + direction;
                                var ni = Mathx.ToIndex(neighbour.x, neighbour.y, width);

                                if (
                                    mapRange.Contains(neighbour) &&
                                    map[ni] == type && !isChecked[ni]
                                )
                                {
                                    toCheck.Enqueue(neighbour);
                                    isChecked[ni] = true;
                                }
                            }
                        }

                        result.Add(region);
                    }
                }
            }

            return result;
        }

        private static List<List<Vector2Int>> RemoveRegionsUnderThreshold(List<List<Vector2Int>> regions, int threshold)
        {
            var removed = new List<List<Vector2Int>>();

            for (int i = regions.Count - 1; i > -1; i--)
            {
                var region = regions[i];

                if (region.Count < threshold)
                {
                    regions.RemoveAt(i);
                    removed.Add(region);
                }
            }

            return removed;
        }

        private static void FlipRegions(List<List<Vector2Int>> regions, bool[] map, int width)
        {
            foreach (var region in regions)
            {
                foreach (var tile in region)
                {
                    int i = Mathx.ToIndex(tile.x, tile.y, width);
                    map[i] = !map[i];
                }
            }
        }

        private static List<List<Vector2Int>> CreateRooms(List<List<Vector2Int>> regions, bool[] map, int width, int height)
        {
            var result = new List<List<Vector2Int>>();

            var isChecked = new bool[width * height];
            var mapRange = new Range2Int(0, 0, width - 1, height - 1);

            foreach (var region in regions)
            {
                var room = new List<Vector2Int>();

                foreach (var tile in region)
                {
                    foreach (var direction in Axes.All2D)
                    {
                        var neighbour = tile + direction;
                        int ni = Mathx.ToIndex(neighbour.x, neighbour.y, width);

                        if (
                            mapRange.Contains(neighbour) &&
                            map[ni] == kWall && !isChecked[ni]
                        )
                        {
                            room.Add(neighbour);
                            isChecked[ni] = true;
                        }
                    }
                }

                result.Add(room);
            }

            return result;
        }

        // TODO: Bottleneck. Find a more optimal way.
        private static List<Line> FindPassages(List<List<Vector2Int>> rooms)
        {
            var result = new List<Line>();

            if (rooms.Count < 1)
                return result;
            
            var roomA = rooms[0];

            while (rooms.Count > 1)
            {
                var line = new Line();
                var distance = int.MaxValue;
                var index = 0;

                for (int i = 1; i < rooms.Count; ++i)
                {
                    var roomB = rooms[i];

                    for (int ta = 0; ta < roomA.Count; ta++)
                    {
                        var tileA = roomA[ta];

                        for (int tb = 0; tb < roomB.Count; tb++)
                        {
                            var tileB = roomB[tb];

                            var dx = tileB.x - tileA.x;
                            var dy = tileB.y - tileA.y;

                            var currentDistance = dx * dx + dy * dy;

                            if (currentDistance < distance)
                            {
                                line.a = tileA;
                                line.b = tileB;

                                distance = currentDistance;

                                index = i;
                            }
                        }
                    }
                }

                roomA.AddRange(rooms[index]);
                rooms.RemoveAt(index);

                result.Add(line);
            }

            return result;
        }

        private static List<Vector2Int> GetTilesOnLine(Vector2Int a, Vector2Int b)
        {
            var result = new List<Vector2Int>();

            int x = a.x;
            int y = a.y;

            int dx = b.x - a.x;
            int dy = b.y - a.y;

            int abs_dx = Math.Abs(dx);
            int abs_dy = Math.Abs(dy);

            int min = Math.Min(abs_dx, abs_dy);
            int max = Math.Max(abs_dx, abs_dy);

            var step = Vector2Int.zero;
            var acc_step = Vector2Int.zero;

            if (abs_dy > abs_dx)
            {
                step.y = Math.Sign(dy);
                acc_step.x = Math.Sign(dx);
            }
            else
            {
                step.x = Math.Sign(dx);
                acc_step.y = Math.Sign(dy);
            }

            int acc = max / 2;

            for (int i = 0; i < max; i++)
            {
                result.Add(new Vector2Int(x, y));

                x += step.x;
                y += step.y;

                acc += min;
                if (acc >= max)
                {
                    x += acc_step.x;
                    y += acc_step.y;

                    acc -= max;
                }
            }
            result.Add(b);

            return result;
        }

        private static void ClearPassages(List<Line> passages, bool[] map, int width, int height, int passageWidth, int borderWidth, int borderHeight)
        {
            foreach (var passage in passages)
            {
                var tiles = GetTilesOnLine(passage.a, passage.b);

                foreach (var tile in tiles)
                {
                    ClearCircle(tile, passageWidth, map, width, height, borderWidth, borderHeight);
                }
            }
        }

        private static void ClearCircle(Vector2Int tile, int r, bool[] map, int width, int height, int borderWidth, int borderHeight)
        {
            var mapRange = new Range2Int(
                Math.Max(borderWidth, 0),
                Math.Max(borderHeight, 0),
                Math.Min(width - borderWidth - 1, width - 1),
                Math.Min(height - borderHeight - 1, height - 1)
            );

            for (int y = -r; y <= r; y++)
            {
                for (int x = -r; x <= r; x++)
                {
                    if (x * x + y * y <= r * r)
                    {
                        var clearTile = new Vector2Int(tile.x + x, tile.y + y);

                        if (mapRange.Contains(clearTile))
                        {
                            var i = Mathx.ToIndex(clearTile.x, clearTile.y, width);
                            map[i] = kRoom;
                        }
                    }
                }
            }
        }
    }
}
