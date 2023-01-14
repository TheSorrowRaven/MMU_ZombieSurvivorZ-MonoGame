using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;

namespace ZombieSurvivorZ
{
    public class TileGraph
    {

        public struct TileData
        {
            public bool walkable;
            public float cost;
        }

        public HashSet<Vector2Int> Nodes;
        public Dictionary<Vector2Int, float[]> Connections;

        private const float Sqr2 = 1.41421356237f;
        private static readonly float[] Cost = { Sqr2, 1, Sqr2, 1, 1, Sqr2, 1, Sqr2 };

        private static Vector2Int MapSize => Game1.MapManager.MapSize;

        public TileGraph()
        {
            Nodes = new();
            Connections = new();
        }

        public void UpdateNodeCost(Vector2Int target, float cost)
        {
            //Updates surrounding tiles' connection to target
            int i = 8;  //Start from 8 (7 to 0)
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    i--;
                    Vector2Int cell = new(target.X + x, target.Y + y);
                    if (!Connections.TryGetValue(cell, out float[] weights))
                    {
                        continue;
                    }
                    weights[i] = Cost[i] + cost;

                }
            }
        }

        public void BFSConstructGraph(Func<ushort, ushort, TileData> getTileDataFunc, int xStart, int yStart)
        {
            Vector2Int start = new(xStart, yStart);
            Nodes.Add(start);

            Queue<Vector2Int> queue = new();
            queue.Enqueue(start);

            while (queue.TryDequeue(out Vector2Int current))
            {
                int i = -1;
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        if (x == 0 && y == 0)
                        {
                            continue;
                        }

                        i++;
                        int yN = current.Y + y;
                        int xN = current.X + x;

                        if (!WithinMapBoundary(xN, yN))
                        {
                            continue;
                        }
                        TileData tileData = getTileDataFunc((ushort)xN, (ushort)yN);
                        if (!tileData.walkable)
                        {
                            //No connections because not walkable
                            continue;
                        }
                        float tileCost = tileData.cost;

                        //TODO Determine walls, and set costs based on barricade health
                        Vector2Int neighbour = new(xN, yN);

                        if (!Nodes.Contains(neighbour))
                        {
                            Nodes.Add(neighbour);
                            queue.Enqueue(neighbour);
                        }

                        if (!Connections.TryGetValue(current, out float[] weights))
                        {
                            weights = new float[Cost.Length];
                            Connections.Add(current, weights);
                        }

                        if (Math.Abs(x * y) == 1) //Is Diagonal
                        {
                            if (tileCost != 0)
                            {
                                continue;
                            }

                            var td = getTileDataFunc((ushort)(xN), (ushort)(current.Y));
                            if (!td.walkable || td.cost != 0)
                            {
                                //No connection, Don't set weight
                                //Console.WriteLine("Skip");
                                continue;
                            }
                            td = getTileDataFunc((ushort)(current.X), (ushort)(yN));
                            if (!td.walkable || td.cost != 0)
                            {
                                //No connection, Don't set weight
                                //Console.WriteLine("Skip");
                                continue;
                            }
                        }

                        weights[i] = Cost[i] + tileCost;
                    }
                }
            }
        }

        private static bool WithinMapBoundary(int x, int y)
        {
            return x >= 0 && x < MapSize.X && y >= 0 && y < MapSize.Y;
        }
    }
}