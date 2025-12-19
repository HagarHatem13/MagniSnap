using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using static MagniSnap.ImageToolkit;

namespace MagniSnap
{
    public partial class MainForm : Form
    {
        RGBPixel[,] ImageMatrix;
        bool isLassoEnabled = false;

        RGBPixel[,] fixedImage;
        RGBPixel[,] tempDisplayImage;

        Vector2D[,] EnergyMap;
        double[,] CostGraph;
        double[,] Dist;
        Point[,] Parent;

        int anchorX = -1;
        int anchorY = -1;
        bool hasAnchor = false;

        private int lastMouseX = -1;
        private int lastMouseY = -1;
        private const int MOUSE_MOVE_THRESHOLD = 3;

        public MainForm()
        {
            InitializeComponent();
            indicator_pnl.Hide();

            mainPictureBox.MouseClick += mainPictureBox_MouseClick;
            mainPictureBox.MouseMove += mainPictureBox_MouseMove;
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            indicator_pnl.Top = ((Control)sender).Top;
            indicator_pnl.Height = ((Control)sender).Height;
            indicator_pnl.Left = ((Control)sender).Left;
            ((Control)sender).BackColor = Color.FromArgb(37, 46, 59);
            indicator_pnl.Show();
        }

        private void menuButton_Leave(object sender, EventArgs e)
        {
            ((Control)sender).BackColor = Color.FromArgb(26, 32, 40);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            ImageMatrix = ImageToolkit.OpenImage(dlg.FileName);
            ImageToolkit.ViewImage(ImageMatrix, mainPictureBox);

            fixedImage = (RGBPixel[,])ImageMatrix.Clone();
            tempDisplayImage = (RGBPixel[,])fixedImage.Clone();

            RGBPixel[,] smooth = LiveWireProcessor.PreprocessImage(ImageMatrix);
            EnergyMap = LiveWireProcessor.ComputeEnergyMap(smooth);
            CostGraph = LiveWireProcessor.BuildCostGraph(EnergyMap);

            txtWidth.Text = ImageToolkit.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageToolkit.GetHeight(ImageMatrix).ToString();

            hasAnchor = false;
            anchorX = anchorY = -1;
            lastMouseX = lastMouseY = -1;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fixedImage == null) return;
            ImageToolkit.ViewImage(fixedImage, mainPictureBox);
            hasAnchor = false;
            anchorX = anchorY = -1;
        }

        // ================== LIVEWIRE ==================

        private void btnLivewire_Click(object sender, EventArgs e)
        {
            menuButton_Click(sender, e);
            mainPictureBox.Cursor = Cursors.Cross;
            isLassoEnabled = true;
        }

        private void btnLivewire_Leave(object sender, EventArgs e)
        {
            menuButton_Leave(sender, e);
            mainPictureBox.Cursor = Cursors.Default;
            isLassoEnabled = false;
        }

        private void mainPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isLassoEnabled || CostGraph == null || fixedImage == null)
                return;

            int w = fixedImage.GetLength(1);
            int h = fixedImage.GetLength(0);

            int fx = Math.Max(0, Math.Min(w - 1, e.X));
            int fy = Math.Max(0, Math.Min(h - 1, e.Y));

            if (!hasAnchor)
            {
                anchorX = fx;
                anchorY = fy;

                LiveWireProcessor.ComputeShortestPaths(
                    CostGraph,
                    anchorX, anchorY,
                    out Dist,
                    out Parent);

                hasAnchor = true;
            }
            else
            {
                var path = LiveWireProcessor.Backtrack(Parent, fx, fy);
                LiveWireProcessor.DrawPath(fixedImage, path);

                anchorX = fx;
                anchorY = fy;

                LiveWireProcessor.ComputeShortestPaths(
                    CostGraph,
                    anchorX, anchorY,
                    out Dist,
                    out Parent);

                // ✅ CHANGED AFTER LIVEWIRE ONLY
                ImageToolkit.ViewImageSafe(fixedImage, mainPictureBox);

                Array.Copy(fixedImage, tempDisplayImage, fixedImage.Length);
            }

            lastMouseX = lastMouseY = -1;
        }

        private void mainPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!hasAnchor || Parent == null || fixedImage == null)
                return;

            int w = fixedImage.GetLength(1);
            int h = fixedImage.GetLength(0);

            int fx = Math.Max(0, Math.Min(w - 1, e.X));
            int fy = Math.Max(0, Math.Min(h - 1, e.Y));

            txtMousePosX.Text = fx.ToString();
            txtMousePosY.Text = fy.ToString();

            if (lastMouseX != -1 &&
                Math.Abs(fx - lastMouseX) < MOUSE_MOVE_THRESHOLD &&
                Math.Abs(fy - lastMouseY) < MOUSE_MOVE_THRESHOLD)
                return;

            lastMouseX = fx;
            lastMouseY = fy;

            Array.Copy(fixedImage, tempDisplayImage, fixedImage.Length);

            var livePath = LiveWireProcessor.Backtrack(Parent, fx, fy);
            LiveWireProcessor.DrawPathOptimized(tempDisplayImage, livePath, 255, 255, 0);

            // ✅ CHANGED AFTER LIVEWIRE ONLY
            ImageToolkit.ViewImageSafe(tempDisplayImage, mainPictureBox);
        }
    }
}