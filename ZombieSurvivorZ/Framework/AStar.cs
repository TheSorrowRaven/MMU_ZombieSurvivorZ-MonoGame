using System;
using System.Collections;
using System.Collections.Generic;


namespace ZombieSurvivorZ
{
    public class AStar
    {

        private class NodeRecord
        {
            public Vector2Int self;
            public Vector2Int? from;
            public float costSoFar;

            public NodeRecord(Vector2Int self, Vector2Int? from, float costSoFar)
            {
                this.self = self;
                this.from = from;
                this.costSoFar = costSoFar;
            }

            public override string ToString()
            {
                return self.ToString();
            }
        }

        private static readonly Dictionary<Vector2Int, NodeRecord> records = new();
        private static readonly PriorityQueue<NodeRecord, float> pQueue = new();

        public static LinkedList<Vector2Int> Compute(TileGraph graph, Vector2Int start, Vector2Int goal)
        {
            records.Clear();
            pQueue.Clear();

            NodeRecord startRecord = new(start, null, 0);

            pQueue.Enqueue(startRecord, 0);
            records[start] = startRecord;

            while (pQueue.TryDequeue(out NodeRecord curRecord, out _))
            {
                Vector2Int currentTile = curRecord.self;

                if (currentTile == goal)
                    break;

                float[] costs = graph.Connections[currentTile];

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
                        if (costs[i] == 0) continue;

                        float newCost = curRecord.costSoFar + costs[i];

                        Vector2Int neighbourTile = new(currentTile.X + x, currentTile.Y + y);

                        bool successGetValue = records.TryGetValue(neighbourTile, out NodeRecord neighbourRecord);

                        bool shouldEnqueue = false;

                        if (successGetValue && newCost < neighbourRecord.costSoFar)
                        {
                            shouldEnqueue = true;
                            neighbourRecord.costSoFar = newCost + Manhattan(goal, neighbourTile);
                        }
                        else if (!successGetValue)
                        {
                            shouldEnqueue = true;
                            neighbourRecord = new(neighbourTile, currentTile, newCost);
                            records[neighbourTile] = neighbourRecord;
                        }

                        if (shouldEnqueue)
                        {
                            float priority = newCost;
                            pQueue.Enqueue(neighbourRecord, priority);
                        }
                    }
                }

            }

            return ConstructPath(records, start, goal);
        }

        private static LinkedList<Vector2Int> ConstructPath(Dictionary<Vector2Int, NodeRecord> nodeRecords, Vector2Int start, Vector2Int goal)
        {
            LinkedList<Vector2Int> path = new();

            for (NodeRecord cur = nodeRecords[goal]; cur.from != null;)
            {
                path.AddFirst(cur.self);
                cur = nodeRecords[cur.from.Value];
            }
            path.AddFirst(start);

            return path;
        }

        private static float Manhattan(Vector2Int newNode, Vector2Int end)
        {
            return Math.Abs(newNode.X - end.X) + Math.Abs(newNode.Y - end.Y);
        }

    }
}