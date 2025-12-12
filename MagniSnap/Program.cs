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

            Console.WriteLine("Test completed successfully!");
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
