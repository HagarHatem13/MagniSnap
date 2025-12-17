using System;
using System.Drawing;
using System.Windows.Forms;
using static MagniSnap.ImageToolkit;

namespace MagniSnap
{
    #region
    /// 4d17639adfad0a300acd78759e07a4f2
    #endregion
    public partial class MainForm : Form
    {
        RGBPixel[,] ImageMatrix;
        bool isLassoEnabled = false;

        RGBPixel[,] fixedImage;
        Vector2D[,] EnergyMap;
        double[,] CostGraph;
        double[,] Dist;
        Point[,] Parent;

        int anchorX = -1;
        int anchorY = -1;
        bool hasAnchor = false;


        public MainForm()
        {
            InitializeComponent();
            indicator_pnl.Hide();
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            #region Do Change Remove Template Code
            /// 4d17639adfad0a300acd78759e07a4f2
            #endregion

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
            #region Do Change Remove Template Code
            /// 4d17639adfad0a300acd78759e07a4f2
            #endregion

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageToolkit.OpenImage(OpenedFilePath);
                ImageToolkit.ViewImage(ImageMatrix, mainPictureBox);
                // add here function struct graph b3d ta3deel 
                ////////////////////////////////////////////////////////////////////////
                fixedImage = (RGBPixel[,])ImageMatrix.Clone();

                RGBPixel[,] smooth =
                    LiveWireProcessor.PreprocessImage(ImageMatrix);

                EnergyMap =
                    LiveWireProcessor.ComputeEnergyMap(smooth);

                CostGraph =
                    LiveWireProcessor.BuildCostGraph(EnergyMap);
                //////////////////////////////////////////////////////

                int width = ImageToolkit.GetWidth(ImageMatrix);
                txtWidth.Text = width.ToString();
                int height = ImageToolkit.GetHeight(ImageMatrix);
                txtHeight.Text = height.ToString();
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainPictureBox.Refresh();
        }

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
            ///////////////////////////////////////////////////////////////////////////
       
            if (CostGraph == null || fixedImage == null)
                return;

            if (!hasAnchor)
            {
                anchorX = e.X;
                anchorY = e.Y;

                LiveWireProcessor.ComputeShortestPaths(
                    CostGraph,
                    anchorX, anchorY,
                    out Dist,
                    out Parent);

                hasAnchor = true;
            }
            else
            {
                var path =
                    LiveWireProcessor.Backtrack(Parent, e.X, e.Y);

                LiveWireProcessor.DrawPath(fixedImage, path);

                anchorX = e.X;
                anchorY = e.Y;

                LiveWireProcessor.ComputeShortestPaths(
                    CostGraph,
                    anchorX, anchorY,
                    out Dist,
                    out Parent);

                ImageToolkit.ViewImage(
                    fixedImage, mainPictureBox);
                ///////////////////////////////////////////////
            }
        }


        private void mainPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            //////////////////////////////////////////////////////////////////////////////
           
            if (ImageMatrix == null || fixedImage == null)
                return;

            txtMousePosX.Text = e.X.ToString();
            txtMousePosY.Text = e.Y.ToString();

            int h = ImageToolkit.GetHeight(ImageMatrix);
            int w = ImageToolkit.GetWidth(ImageMatrix);

            int fx = e.X;
            int fy = e.Y;

            if (fx < 0) fx = 0;
            if (fy < 0) fy = 0;
            if (fx >= w) fx = w - 1;
            if (fy >= h) fy = h - 1;

           
            if (hasAnchor && CostGraph != null)
            {
                if (Parent == null)
                {
                    LiveWireProcessor.ComputeShortestPaths(
                        CostGraph,
                        anchorX, anchorY,
                        out Dist,
                        out Parent);
                }
                ///////////////////////////////////////////////////////////////////////

                RGBPixel[,] temp = (RGBPixel[,])fixedImage.Clone();

                var livePath = LiveWireProcessor.Backtrack(Parent, fx, fy);
                LiveWireProcessor.DrawPath(temp, livePath);

                ImageToolkit.ViewImage(temp, mainPictureBox);
            }
        

           
        
            if (ImageMatrix != null && isLassoEnabled)
            {
                // Refresh to redraw points
                mainPictureBox.Refresh();
            }
        }
    }
}
