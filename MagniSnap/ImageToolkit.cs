using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MagniSnap
{


    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    public struct RGBPixel
    {
        public byte red, green, blue;
    }

    public struct RGBPixelD
    {
        public double red, green, blue;
    }

    /// <summary>
    /// Holds the edge energy between 
    ///     1. a pixel and its right one (X)
    ///     2. a pixel and its bottom one (Y)
    /// </summary>
    public struct Vector2D
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    /// <summary>
    /// Library of static functions that deal with images
    /// </summary>
    //Salma&Dania
    public class PixelGraph
    {
        public int Width { get; }
        public int Height { get; }

        // Graph[nodeId] = list of (neighborId, weight)
        public List<(int neighborId, double weight)>[] Graph { get; }

        public PixelGraph(int width, int height)
        {
            if (width <= 0) throw new ArgumentException(nameof(width));
            if (height <= 0) throw new ArgumentException(nameof(height));

            Width = width;
            Height = height;

            int totalNodes = width * height;

            Graph = new List<(int, double)>[totalNodes];

            for (int i = 0; i < totalNodes; i++)
                Graph[i] = new List<(int, double)>();
        }
        public class PixelGraphBuilder
        {
            public PixelGraph BuildGraph(RGBPixel[,] ImageMatrix)
            {
                int H = ImageToolkit.GetHeight(ImageMatrix);
                int W = ImageToolkit.GetWidth(ImageMatrix);

                PixelGraph graph = new PixelGraph(W, H);

                // Loop through all pixels
                for (int y = 0; y < H; y++)
                {
                    for (int x = 0; x < W; x++)
                    {
                        int currentID = y * W + x;

                        // TEMPLATE FUNCTION — returns edge energy to RIGHT (X) and DOWN (Y)
                        Vector2D e = ImageToolkit.CalculatePixelEnergies(x, y, ImageMatrix);

                        // Add RIGHT neighbor (x+1, y)
                        if (x + 1 < W)
                        {
                            int rightID = y * W + (x + 1);
                            double weight = 1.0 / (e.X + 0.000001); // convert energy -> weight
                            graph.AddUndirectedEdge(currentID, rightID, weight);
                        }

                        // Add DOWN neighbor (x, y+1)
                        if (y + 1 < H)
                        {
                            int downID = (y + 1) * W + x;
                            double weight = 1.0 / (e.Y + 0.000001);
                            graph.AddUndirectedEdge(currentID, downID, weight);
                        }
                    }
                }

                return graph;
            }
        }


        /// <summary>
        /// Add UNDIRECTED edge (adds both A->B and B->A).
        ///</summary>
        public void AddUndirectedEdge(int u, int v, double weight)
        {
            Graph[u].Add((v, weight));
            Graph[v].Add((u, weight));
        }

       
        private class MinHeap
        {
            private List<(int vertex, double distance)> heap;
            private int size;

            public MinHeap()
            {
                heap = new List<(int, double)>();
                size = 0;
            }

            public int Count => size;

            public bool IsEmpty => size == 0;

            public void Insert(int vertex, double distance)
            {
                if (size < heap.Count)
                {
                    heap[size] = (vertex, distance);
                }
                else
                {
                    heap.Add((vertex, distance));
                }
                size++;
                HeapifyUp(size - 1);
            }

            public (int vertex, double distance) ExtractMin()
            {
                if (size == 0)
                    throw new InvalidOperationException("Heap is empty");

                (int vertex, double distance) min = heap[0];
                heap[0] = heap[size - 1];
                size--;

                if (size > 0)
                    HeapifyDown(0);

                return min;
            }

            private void HeapifyUp(int index)
            {
                while (index > 0)
                {
                    int parent = (index - 1) / 2;
                    if (heap[parent].distance <= heap[index].distance)
                        break;

                    Swap(parent, index);
                    index = parent;
                }
            }

            private void HeapifyDown(int index)
            {
                while (true)
                {
                    int smallest = index;
                    int left = 2 * index + 1;
                    int right = 2 * index + 2;

                    if (left < size && heap[left].distance < heap[smallest].distance)
                        smallest = left;

                    if (right < size && heap[right].distance < heap[smallest].distance)
                        smallest = right;

                    if (smallest == index)
                        break;

                    Swap(index, smallest);
                    index = smallest;
                }
            }

            private void Swap(int i, int j)
            {
                var temp = heap[i];
                heap[i] = heap[j];
                heap[j] = temp;
            }
        }

        /// <summary>
        /// Calculate shortest paths from an anchor pixel to all pixels using Dijkstra's algorithm.
        /// Time complexity: O(E' lg(V')) where V' and E' are vertices and edges checked until reaching destination.
        /// </summary>
        /// <param name="anchorPixelId">The ID of the anchor pixel (vertex)</param>
        /// <returns>
        /// A tuple containing:
        /// - distances: array of shortest distances from anchor to each pixel (double.MaxValue if unreachable)
        /// - parents: array of parent vertex IDs for path reconstruction (-1 if no parent)
        /// </returns>
        public (double[] distances, int[] parents) CalculateShortestPathsFromAnchor(int anchorPixelId)
        {
            int totalNodes = Width * Height;
            
            if (anchorPixelId < 0 || anchorPixelId >= totalNodes)
                throw new ArgumentException("Invalid anchor pixel ID", nameof(anchorPixelId));

            // Initialize distances and parents
            double[] distances = new double[totalNodes];
            int[] parents = new int[totalNodes];
            bool[] visited = new bool[totalNodes];

            for (int i = 0; i < totalNodes; i++)
            {
                distances[i] = double.MaxValue;
                parents[i] = -1;
                visited[i] = false;
            }

            distances[anchorPixelId] = 0.0;

            // Use min-heap as priority queue
            MinHeap priorityQueue = new MinHeap();
            priorityQueue.Insert(anchorPixelId, 0.0);

            while (!priorityQueue.IsEmpty)
            {
                // Extract minimum distance vertex
                var (u, dist) = priorityQueue.ExtractMin();

                // Skip if already processed or if this entry is stale (distance doesn't match)
                if (visited[u] || dist > distances[u])
                    continue;

                visited[u] = true;

                // Relax all neighbors
                foreach (var (neighborId, weight) in Graph[u])
                {
                    if (!visited[neighborId])
                    {
                        double newDistance = distances[u] + weight;

                        if (newDistance < distances[neighborId])
                        {
                            distances[neighborId] = newDistance;
                            parents[neighborId] = u;

                            // Add new entry (duplicates are allowed, we'll skip stale ones)
                            priorityQueue.Insert(neighborId, newDistance);
                        }
                    }
                }
            }

            return (distances, parents);
        }

        /// <summary>
        /// Backtrack the shortest path from a free point (mouse position) to the anchor point.
        /// Time complexity: O(N) where N is the length of the path.
        /// </summary>
        /// <param name="freePointId">The ID of the free point (mouse position pixel)</param>
        /// <param name="parents">The parent array from CalculateShortestPathsFromAnchor</param>
        /// <param name="anchorPixelId">The ID of the anchor pixel</param>
        /// <returns>
        /// A list of pixel IDs representing the path from freePointId to anchorPixelId.
        /// Returns empty list if no path exists.
        /// </returns>
        public List<int> BacktrackPath(int freePointId, int[] parents, int anchorPixelId)
        {
            List<int> path = new List<int>();

            if (freePointId < 0 || freePointId >= Width * Height)
                return path;

            if (parents == null || parents.Length != Width * Height)
                throw new ArgumentException("Invalid parents array", nameof(parents));

            // Backtrack from free point to anchor
            int current = freePointId;
            bool reachedAnchor = false;

            while (current != -1)
            {
                path.Add(current);

                if (current == anchorPixelId)
                {
                    reachedAnchor = true;
                    break;
                }

                current = parents[current];
            }

            // If we didn't reach the anchor, no valid path exists
            if (!reachedAnchor)
                path.Clear();

            return path;
        }

        /// <summary>
        /// Convert pixel coordinates (x, y) to pixel ID
        /// </summary>
        public int PixelToId(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                throw new ArgumentException("Invalid pixel coordinates");
            return y * Width + x;
        }

        /// <summary>
        /// Convert pixel ID to pixel coordinates (x, y)
        /// </summary>
        public (int x, int y) IdToPixel(int pixelId)
        {
            if (pixelId < 0 || pixelId >= Width * Height)
                throw new ArgumentException("Invalid pixel ID");
            int y = pixelId / Width;
            int x = pixelId % Width;
            return (x, y);
        }
    }

    public class ImageToolkit
    {
        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        public static RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            RGBPixel[,] Buffer = new RGBPixel[Height, Width];

            unsafe
            {
                #region Do Change Remove Template Code
                /// 08e850689d67340abacf2bc76d98212d
                #endregion
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                #region Do Change Remove Template Code
                /// 08e850689d67340abacf2bc76d98212d
                #endregion
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[0];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[2];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }

        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        /// <summary>
        /// Calculate edge energy between
        ///     1. the given pixel and its right one (X)
        ///     2. the given pixel and its bottom one (Y)
        /// </summary>
        /// <param name="x">pixel x-coordinate</param>
        /// <param name="y">pixel y-coordinate</param>
        /// <param name="ImageMatrix">colored image matrix</param>
        /// <returns>edge energy with the right pixel (X) and with the bottom pixel (Y)</returns>
        public static Vector2D CalculatePixelEnergies(int x, int y, RGBPixel[,] ImageMatrix)
        {
            if (ImageMatrix == null) throw new Exception("image is not set!");

            Vector2D gradient = CalculateGradientAtPixel(x, y, ImageMatrix);
            #region Do Change Remove Template Code
            /// 08e850689d67340abacf2bc76d98212d
            #endregion
            double gradientMagnitude = Math.Sqrt(gradient.X * gradient.X + gradient.Y * gradient.Y);
            double edgeAngle = Math.Atan2(gradient.Y, gradient.X);
            double rotatedEdgeAngle = edgeAngle + Math.PI / 2.0;
            #region Do Change Remove Template Code
            /// 08e850689d67340abacf2bc76d98212d
            #endregion
            Vector2D energy = new Vector2D();
            energy.X = Math.Abs(gradientMagnitude * Math.Cos(rotatedEdgeAngle));
            energy.Y = Math.Abs(gradientMagnitude * Math.Sin(rotatedEdgeAngle));

            return energy;
        }

        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void ViewImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                #region Do Change Remove Template Code
                /// 08e850689d67340abacf2bc76d98212d
                #endregion
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    #region Do Change Remove Template Code
                    /// 08e850689d67340abacf2bc76d98212d
                    #endregion
                    for (int j = 0; j < Width; j++)
                    {
                        p[0] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[2] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }


        /// <summary>
        /// Apply Gaussian smoothing filter to enhance the edge detection 
        /// </summary>
        /// <param name="ImageMatrix">Colored image matrix</param>
        /// <param name="filterSize">Gaussian mask size</param>
        /// <param name="sigma">Gaussian sigma</param>
        /// <returns>smoothed color image</returns>
        public static RGBPixel[,] GaussianFilter1D(RGBPixel[,] ImageMatrix, int filterSize, double sigma)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);

            RGBPixelD[,] VerFiltered = new RGBPixelD[Height, Width];
            RGBPixel[,] Filtered = new RGBPixel[Height, Width];


            // Create Filter in Spatial Domain:
            //=================================
            //make the filter ODD size
            if (filterSize % 2 == 0) filterSize++;

            double[] Filter = new double[filterSize];
            #region Do Change Remove Template Code
            /// 08e850689d67340abacf2bc76d98212d
            #endregion
            //Compute Filter in Spatial Domain :
            //==================================
            double Sum1 = 0;
            int HalfSize = filterSize / 2;
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                //Filter[y+HalfSize] = (1.0 / (Math.Sqrt(2 * 22.0/7.0) * Segma)) * Math.Exp(-(double)(y*y) / (double)(2 * Segma * Segma)) ;
                Filter[y + HalfSize] = Math.Exp(-(double)(y * y) / (double)(2 * sigma * sigma));
                Sum1 += Filter[y + HalfSize];
            }
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                Filter[y + HalfSize] /= Sum1;
            }
            #region Do Change Remove Template Code
            /// 08e850689d67340abacf2bc76d98212d
            #endregion
            //Filter Original Image Vertically:
            //=================================
            int ii, jj;
            RGBPixelD Sum;
            RGBPixel Item1;
            RGBPixelD Item2;

            for (int j = 0; j < Width; j++)
                for (int i = 0; i < Height; i++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int y = -HalfSize; y <= HalfSize; y++)
                    {
                        ii = i + y;
                        if (ii >= 0 && ii < Height)
                        {
                            Item1 = ImageMatrix[ii, j];
                            Sum.red += Filter[y + HalfSize] * Item1.red;
                            Sum.green += Filter[y + HalfSize] * Item1.green;
                            Sum.blue += Filter[y + HalfSize] * Item1.blue;
                        }
                    }
                    VerFiltered[i, j] = Sum;
                }
            #region Do Change Remove Template Code
            /// 08e850689d67340abacf2bc76d98212d
            #endregion
            //Filter Resulting Image Horizontally:
            //===================================
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int x = -HalfSize; x <= HalfSize; x++)
                    {
                        jj = j + x;
                        if (jj >= 0 && jj < Width)
                        {
                            Item2 = VerFiltered[i, jj];
                            Sum.red += Filter[x + HalfSize] * Item2.red;
                            Sum.green += Filter[x + HalfSize] * Item2.green;
                            Sum.blue += Filter[x + HalfSize] * Item2.blue;
                        }
                    }
                    Filtered[i, j].red = (byte)Sum.red;
                    Filtered[i, j].green = (byte)Sum.green;
                    Filtered[i, j].blue = (byte)Sum.blue;
                }

            return Filtered;
        }


        #region Private Functions
        /// <summary>
        /// Calculate Gradient vector between the given pixel and its right and bottom ones
        /// </summary>
        /// <param name="x">pixel x-coordinate</param>
        /// <param name="y">pixel y-coordinate</param>
        /// <param name="ImageMatrix">colored image matrix</param>
        /// <returns></returns>
        private static Vector2D CalculateGradientAtPixel(int x, int y, RGBPixel[,] ImageMatrix)
        {
            Vector2D gradient = new Vector2D();

            RGBPixel mainPixel = ImageMatrix[y, x];
            double pixelGrayVal = 0.21 * mainPixel.red + 0.72 * mainPixel.green + 0.07 * mainPixel.blue;

            if (y == GetHeight(ImageMatrix) - 1)
            {
                //boundary pixel.
                for (int i = 0; i < 3; i++)
                {
                    gradient.Y = 0;
                }
            }
            else
            {
                RGBPixel downPixel = ImageMatrix[y + 1, x];
                double downPixelGrayVal = 0.21 * downPixel.red + 0.72 * downPixel.green + 0.07 * downPixel.blue;

                gradient.Y = pixelGrayVal - downPixelGrayVal;
            }
            #region Do Change Remove Template Code
            /// 08e850689d67340abacf2bc76d98212d
            #endregion
            if (x == GetWidth(ImageMatrix) - 1)
            {
                //boundary pixel.
                gradient.X = 0;

            }
            else
            {
                RGBPixel rightPixel = ImageMatrix[y, x + 1];
                double rightPixelGrayVal = 0.21 * rightPixel.red + 0.72 * rightPixel.green + 0.07 * rightPixel.blue;

                gradient.X = pixelGrayVal - rightPixelGrayVal;
            }
            
            return gradient;
        }


        #endregion
    }

}
