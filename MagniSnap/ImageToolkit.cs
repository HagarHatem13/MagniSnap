using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using MagniSnap;

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

        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

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

        public static void ViewImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
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

      
        public static RGBPixel[,] GaussianFilter1D(RGBPixel[,] ImageMatrix, int filterSize, double sigma)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);

            RGBPixelD[,] VerFiltered = new RGBPixelD[Height, Width];
            RGBPixel[,] Filtered = new RGBPixel[Height, Width];

            if (filterSize % 2 == 0) filterSize++;

            double[] Filter = new double[filterSize];
            #region Do Change Remove Template Code
            /// 08e850689d67340abacf2bc76d98212d
            #endregion
            double Sum1 = 0;
            int HalfSize = filterSize / 2;
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
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
        private static Vector2D CalculateGradientAtPixel(int x, int y, RGBPixel[,] ImageMatrix)
        {
            Vector2D gradient = new Vector2D();

            RGBPixel mainPixel = ImageMatrix[y, x];
            double pixelGrayVal = 0.21 * mainPixel.red + 0.72 * mainPixel.green + 0.07 * mainPixel.blue;

            if (y == GetHeight(ImageMatrix) - 1)
            {
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

        public static class LiveWireProcessor
        {
            private const int MAX_RADIUS = 400; // safe, adjustable
            private static double[,] distBuf;
            private static Point[,] parentBuf;
            private static bool[,] visitedBuf;
            private static int bufH = -1, bufW = -1;

            private struct Node
            {
                public int x, y;
                public double d;
                public Node(int x, int y, double d)
                {
                    this.x = x; this.y = y; this.d = d;
                }
            }

            private class MinHeap
            {
                private List<Node> h = new List<Node>();
                public int Count => h.Count;

                public void Push(Node n)
                {
                    h.Add(n);
                    int i = h.Count - 1;
                    while (i > 0)
                    {
                        int p = (i - 1) / 2;
                        if (h[p].d <= h[i].d) break;
                        Swap(i, p);
                        i = p;
                    }
                }

                public Node Pop()
                {
                    Node r = h[0];
                    h[0] = h[h.Count - 1];
                    h.RemoveAt(h.Count - 1);
                    Heapify(0);
                    return r;
                }

                private void Heapify(int i)
                {
                    while (true)
                    {
                        int l = 2 * i + 1, r = 2 * i + 2, s = i;
                        if (l < h.Count && h[l].d < h[s].d) s = l;
                        if (r < h.Count && h[r].d < h[s].d) s = r;
                        if (s == i) break;
                        Swap(i, s);
                        i = s;
                    }
                }

                private void Swap(int a, int b)
                {
                    Node t = h[a];
                    h[a] = h[b];
                    h[b] = t;
                }
            }

            public static RGBPixel[,] PreprocessImage(RGBPixel[,] image)
            {
                return ImageToolkit.GaussianFilter1D(image, 5, 1.0);
            }

            public static Vector2D[,] ComputeEnergyMap(RGBPixel[,] image)
            {
                int h = ImageToolkit.GetHeight(image);
                int w = ImageToolkit.GetWidth(image);
                Vector2D[,] map = new Vector2D[h, w];

                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        map[y, x] = ImageToolkit.CalculatePixelEnergies(x, y, image);

                return map;
            }

            public static double[,] BuildCostGraph(Vector2D[,] energyMap)
            {
                int h = energyMap.GetLength(0);
                int w = energyMap.GetLength(1);
                double[,] cost = new double[h, w];

                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        cost[y, x] = 1.0 / (energyMap[y, x].X + energyMap[y, x].Y + 0.0001);

                return cost;
            }

            public static void ComputeShortestPaths(
                double[,] cost,
                int anchorX, int anchorY,
                out double[,] dist,
                out Point[,] parent)
            {
                int h = cost.GetLength(0);
                int w = cost.GetLength(1);

                if (h != bufH || w != bufW)
                {
                    distBuf = new double[h, w];
                    parentBuf = new Point[h, w];
                    visitedBuf = new bool[h, w];
                    bufH = h; bufW = w;
                }

                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        distBuf[y, x] = double.MaxValue;
                        parentBuf[y, x] = Point.Empty;
                        visitedBuf[y, x] = false;
                    }

                distBuf[anchorY, anchorX] = 0;

                int minX = Math.Max(0, anchorX - MAX_RADIUS);
                int maxX = Math.Min(w - 1, anchorX + MAX_RADIUS);
                int minY = Math.Max(0, anchorY - MAX_RADIUS);
                int maxY = Math.Min(h - 1, anchorY + MAX_RADIUS);

                MinHeap pq = new MinHeap();
                pq.Push(new Node(anchorX, anchorY, 0));

                int[] dx = { -1, 1, 0, 0 };
                int[] dy = { 0, 0, -1, 1 };

                while (pq.Count > 0)
                {
                    Node n = pq.Pop();
                    int x = n.x;
                    int y = n.y;

                    if (visitedBuf[y, x]) continue;
                    visitedBuf[y, x] = true;

                    for (int i = 0; i < 4; i++)
                    {
                        int nx = x + dx[i];
                        int ny = y + dy[i];

                        if (nx < minX || nx > maxX || ny < minY || ny > maxY)
                            continue;

                        if (visitedBuf[ny, nx]) continue;

                        double nd = distBuf[y, x] + cost[ny, nx];

                        if (nd < distBuf[ny, nx])
                        {
                            distBuf[ny, nx] = nd;
                            parentBuf[ny, nx] = new Point(x, y);
                            pq.Push(new Node(nx, ny, nd));
                        }
                    }
                }

                dist = distBuf;
                parent = parentBuf;
            }

            public static List<Point> Backtrack(Point[,] parent, int x, int y)
            {
                List<Point> path = new List<Point>();

                while (parent[y, x] != Point.Empty)
                {
                    path.Add(new Point(x, y));
                    Point p = parent[y, x];
                    x = p.X;
                    y = p.Y;
                }

                return path;
            }

            public static void DrawPathOptimized(RGBPixel[,] img, List<Point> path, byte r, byte g, byte b)
            {
                int h = img.GetLength(0);
                int w = img.GetLength(1);

                foreach (var p in path)
                {
                    if (p.Y >= 0 && p.Y < h && p.X >= 0 && p.X < w)
                    {
                        img[p.Y, p.X].red = r;
                        img[p.Y, p.X].green = g;
                        img[p.Y, p.X].blue = b;
                    }
                }
            }

            public static void DrawPath(RGBPixel[,] img, List<Point> path)
            {
                DrawPathOptimized(img, path, 255, 0, 0);
            }
        }
        public static void ViewImageSafe(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            Image old = PicBox.Image;
            ViewImage(ImageMatrix, PicBox);

            if (old != null && !ReferenceEquals(old, PicBox.Image))
                old.Dispose();
        }
    }
}


