using System.Collections.Generic;

//Original code: https://github.com/mburst/dijkstras-algorithm/blob/master/dijkstras.cs
namespace Extension.Algorithms {
  public class DijkstraGraph {
    Dictionary<string, Dictionary<string, int>> vertices = new Dictionary<string, Dictionary<string, int>>();

    public void AddVertex(string vertexName, Dictionary<string, int> edges) {
      vertices[vertexName] = edges;
    }

    public List<string> FindShortestPath(string start, string finish) {
      var previous = new Dictionary<string, string>();
      var distances = new Dictionary<string, int>();
      var nodes = new List<string>();

      List<string> path = null;

      foreach (var vertex in vertices) {
        distances[vertex.Key] = vertex.Key == start ? 0 : int.MaxValue;
        nodes.Add(vertex.Key);
      }

      while (nodes.Count != 0) {
        nodes.Sort((x, y) => distances[x] - distances[y]);

        var smallest = nodes[0];
        nodes.Remove(smallest);

        if (smallest == finish) {
          path = new List<string>();
          while (previous.ContainsKey(smallest)) {
            path.Add(smallest);
            smallest = previous[smallest];
          }
          break;
        }

        if (distances[smallest] == int.MaxValue)
          break;        

        foreach (var neighbor in vertices[smallest]) {
          var alt = distances[smallest] + neighbor.Value;
          if (alt < distances[neighbor.Key]) {
            distances[neighbor.Key] = alt;
            previous[neighbor.Key] = smallest;
          }
        }
      }

      return path;
    }
  }
}
