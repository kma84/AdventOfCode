
namespace AoCUtils
{
    public static class GraphExtensions
    {

        public static List<T> DijkstraShortestPath<T>(List<(T startNode, T endNode, long cost)> edges, T source, T target) 
            where T : class
        {
            Dictionary<T, long> dist = new() { { source, 0 } };
            Dictionary<T, T?> prev = new() { { source, default } };
            PriorityQueue<T, long> priorityQueue = new();

            Dictionary<T, Dictionary<T, long>> graph = GetGraph(edges);
            priorityQueue.Enqueue(source, dist[source]);

            while (priorityQueue.Count > 0)
            {
                T currentNode = priorityQueue.Dequeue();

                if (currentNode == target)
                    break;

                foreach (T adjacentNode in graph[currentNode].Keys)
                {
                    long tryDist = dist[currentNode] + graph[currentNode][adjacentNode];

                    if (!dist.ContainsKey(adjacentNode) || tryDist < dist[adjacentNode])
                    {
                        dist[adjacentNode] = tryDist;
                        prev[adjacentNode] = currentNode;
                        priorityQueue.Enqueue(adjacentNode, dist[adjacentNode]);
                    }
                }
            }

            Stack<T> path = new();
            T? currentPathNode = target;

            while (currentPathNode != default)
            {
                path.Push(currentPathNode);
                currentPathNode = prev[currentPathNode];
            }

            return path.ToList();
        }

        private static Dictionary<T, Dictionary<T, long>> GetGraph<T>(List<(T startNode, T endNode, long cost)> edges) where T : class
        {
            Dictionary<T, Dictionary<T, long>> graph = new();

            foreach (var (startNode, endNode, cost) in edges)
            {
                if (!graph.ContainsKey(startNode))
                {
                    graph[startNode] = new Dictionary<T, long>();
                }

                graph[startNode][endNode] = cost;
            }

            return graph;
        }

        public static void DijkstraShortestPaths()
        {
            throw new NotImplementedException();
        }

    }
}
