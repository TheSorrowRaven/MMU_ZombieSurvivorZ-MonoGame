using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;

namespace ZombieSurvivorZ
{
    public class TileGraph
    {
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

        public void CreateFromMap(TiledMapTileLayer mapLayer, int xStart, int yStart)
        {
            bool hasTile = mapLayer.TryGetTile((ushort)xStart, (ushort)yStart, out _);

            if (hasTile)
            {
                BFSConstructGraph(mapLayer, xStart, yStart);
            }
            else
            {
                throw new Exception("Error: ColStart or RowStart is Invalid.");
            }
        }

        private void BFSConstructGraph(TiledMapTileLayer mapLayer, int xStart, int yStart)
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
                    for (int x = -1; x < 1; x++)
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
                        if (!mapLayer.TryGetTile((ushort)xN, (ushort)yN, out TiledMapTile? neighbourTile))
                        {
                            continue;
                        }

                        //TODO Determine walls, and set costs based on barricade health
                        if (neighbourTile.Value.GlobalIdentifier == 0)
                        {
                            Vector2Int neighbour = new(xN, yN);

                            if (!Nodes.Contains(neighbour))
                            {
                                Nodes.Add(neighbour);
                                queue.Enqueue(neighbour);
                            }

                            if (Connections.TryGetValue(current, out float[] weights))
                            {
                                weights[i] = Cost[i];
                            }
                            else
                            {
                                weights = new float[Cost.Length];
                                weights[i] = Cost[i];
                                Connections.Add(current, weights);
                            }
                        }
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