using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MagniSnap.PixelGraph;

namespace MagniSnap
{
    #region Do Change Remove Template Code
    /// b95788b0cfef6c04d41189d40f9a1771
    #endregion

    static class Program
    {
        #region Do Change Remove Template Code
        /// b95788b0cfef6c04d41189d40f9a1771
        #endregion
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Create a small 3x3 test image
            RGBPixel[,] img = new RGBPixel[3, 3]
            {
    { new RGBPixel{red=10,green=20,blue=30},
      new RGBPixel{red=50,green=60,blue=70},
      new RGBPixel{red=90,green=100,blue=110} },

    { new RGBPixel{red=15,green=25,blue=35},
      new RGBPixel{red=55,green=65,blue=75},
      new RGBPixel{red=95,green=105,blue=115} },

    { new RGBPixel{red=20,green=30,blue=40},
      new RGBPixel{red=60,green=70,blue=80},
      new RGBPixel{red=100,green=110,blue=120} }
            };

            // Build the graph
            PixelGraphBuilder builder = new PixelGraphBuilder();
            PixelGraph graph = builder.BuildGraph(img);

            // Print the results
            Console.WriteLine($"Graph built with {graph.Graph.Length} nodes.");

            for (int node = 0; node < graph.Graph.Length; node++)
            {
                Console.Write($"Node {node}: ");

                foreach (var (neighborId, weight) in graph.Graph[node])
                {
                    Console.Write($"(to {neighborId}, w={weight:F4}) ");
                }

                Console.WriteLine();
            }

            // Test Dijkstra's algorithm: Calculate shortest paths from anchor pixel
            Console.WriteLine("\n=== Testing Dijkstra's Algorithm ===");
            
            // Set anchor pixel at position (0, 0) - pixel ID 0
            int anchorPixelId = 0;
            Console.WriteLine($"Calculating shortest paths from anchor pixel {anchorPixelId} (coordinates: {graph.IdToPixel(anchorPixelId)})...");
            
            var (distances, parents) = graph.CalculateShortestPathsFromAnchor(anchorPixelId);
            
            Console.WriteLine("\nShortest distances from anchor:");
            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[i] < double.MaxValue)
                {
                    var (x, y) = graph.IdToPixel(i);
                    Console.WriteLine($"  Pixel {i} ({x}, {y}): distance = {distances[i]:F6}");
                }
            }

            // Test backtracking: Find path from a free point to anchor
            Console.WriteLine("\n=== Testing Path Backtracking ===");
            
            // Test backtracking from pixel at position (2, 2) - pixel ID 8
            int freePointId = 8;
            var (freeX, freeY) = graph.IdToPixel(freePointId);
            Console.WriteLine($"Backtracking path from free point {freePointId} ({freeX}, {freeY}) to anchor {anchorPixelId}...");
            
            List<int> path = graph.BacktrackPath(freePointId, parents, anchorPixelId);
            
            if (path.Count > 0)
            {
                Console.WriteLine($"Path found with {path.Count} pixels:");
                foreach (int pixelId in path)
                {
                    var (x, y) = graph.IdToPixel(pixelId);
                    Console.WriteLine($"  Pixel {pixelId}: ({x}, {y})");
                }
            }
            else
            {
                Console.WriteLine("No path found from free point to anchor.");
            }

            // Test with another free point
            int freePointId2 = 4; // Center pixel (1, 1)
            var (freeX2, freeY2) = graph.IdToPixel(freePointId2);
            Console.WriteLine($"\nBacktracking path from free point {freePointId2} ({freeX2}, {freeY2}) to anchor {anchorPixelId}...");
            
            List<int> path2 = graph.BacktrackPath(freePointId2, parents, anchorPixelId);
            
            if (path2.Count > 0)
            {
                Console.WriteLine($"Path found with {path2.Count} pixels:");
                foreach (int pixelId in path2)
                {
                    var (x, y) = graph.IdToPixel(pixelId);
                    Console.WriteLine($"  Pixel {pixelId}: ({x}, {y})");
                }
            }
            else
            {
                Console.WriteLine("No path found from free point to anchor.");
            }

            Console.WriteLine("\nTest completed successfully!");
            Console.WriteLine("Press any key to continue to the application...");
            Console.Read();
        
        #region Do Change Remove Template Code
        /// b95788b0cfef6c04d41189d40f9a1771
        #endregion
        Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
