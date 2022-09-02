
namespace AdventOfCode.Utils
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

            return GetPath(prev, target);
        }

        public static List<T> AStarShortestPath<T>(List<(T startNode, T endNode, long cost)> edges, T source, T target, Func<T, long> h)
            where T : class
        {
            PriorityQueue<T, long> openSet = new();
            Dictionary<T, T?> prev = new() { { source, default } };
            Dictionary<T, long> dist = new() { { source, 0 } };
            Dictionary<T, long> hdist = new() { { source, h(source) } };

            Dictionary<T, Dictionary<T, long>> graph = GetGraph(edges);
            openSet.Enqueue(source, 0);

            while (openSet.Count > 0)
            {
                T currentNode = openSet.Dequeue();

                if (currentNode == target)
                {
                    return GetPath(prev, currentNode);
                }

                foreach (T adjacentNode in graph[currentNode].Keys)
                {
                    long tryDist = dist[currentNode] + graph[currentNode][adjacentNode];

                    if (!dist.ContainsKey(adjacentNode) || tryDist < dist[adjacentNode])
                    {
                        prev[adjacentNode] = currentNode;
                        dist[adjacentNode] = tryDist;
                        hdist[adjacentNode] = tryDist + h(adjacentNode);
                        
                        openSet.Enqueue(adjacentNode, dist[adjacentNode]);
                    }
                }
            }

            return new List<T>();
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

        private static List<T> GetPath<T>(Dictionary<T, T?> prev, T target) where T : class
        {
            Stack<T> path = new();
            T? currentPathNode = target;

            while (currentPathNode != default)
            {
                path.Push(currentPathNode);
                currentPathNode = prev[currentPathNode];
            }

            return path.ToList();
        }

    }
}
