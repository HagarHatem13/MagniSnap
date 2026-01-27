ALGORITHMS
DOCUMENTATION
MAGNISNAP PROJECT
• Openimage FUNCTION
public static RGBPixel[,] Openimage(string ImagePath)
Purpose: This function loads an image from a file and converts it into a format your seam
carving program can work with, can read different image file formats (JPEG, PNG, BMP, etc.) and
different color formats, then gives you a clean, standardized grid of RGB pixels to work with
INPUT:
the file path to your image
OUTPUT:
A 2D grid of RGB pixels that your seam carving algorithms can process directly.
• GET_HEIGHT FUNCTION
public static int GetHeight(RGBPixel [,] ImageMatrix)
Purpose: Returns the height of the image, defined as the number of rows in the image matrix.
Input:
pixels
Parameter is ImageMatrix, it's type is RGBPixel[] and it is a 2D array representing the image
Output:
Return type: int
Description: The number of rows (image height)
Function Description: This function retrieves the size of the first dimension of the 2D image
array using the built-in GetLength(0) method. It does not perform any image processing and
simply returns metadata information about the image.
• GET_WIDTH FUNCTION
public static int GetWidth(RGBPixel[,] ImageMatrix)
Purpose: Returns the width of the image, defined as the number of columns in the image
matrix.
Input:
pixels
Parameter is ImageMatrix it's type is RGBPixel|] and it is a 2D array representing the image
Output:
Return type: int
Description: The number of rows (image height)
Function Description
This function retrieves the size of the second dimension of the image matrix using GetLength (1).
It allows the program to determine how many pixels exist in each row of the image.
• CalculatePixelEnergies FUNCTION
public static Vector2D CalculatePixelEnergies(
int x, int y, RGBPixel[,] ImageMatrix)
Purpose: Calculates the seam carving energy for a single pixel in an image. This energy
represents how "important" a pixel is for preserving image content, where high energy indicates
pixels on edges that should be preserved, and low energy indicates uniform areas that can be
removed.
INPUT:
You need to give this function three pieces of information:
• The x-coordinate - a whole number telling which column the pixel is in, starting from 0
at the left edge
• The y-coordinate - a whole number telling which row the pixel is in, starting from 0 at
the top
• edge
• The image data - a 2D grid of colored pixels (RGB values) that makes up the entire image
The coordinates must be within the image boundaries: x must be less than the image width, and
y must be less than the image height.
OUTPUT:
The function returns an energy vector that contains two numbers:
• The X component - This represents the energy or "removal cost" if this pixel were part of
a vertical seam. A high value means removing this pixel in a vertical seam would create
visible artifacts.
• The Y component - This represents the energy or "removal cost" if this pixel were part of
a horizontal seam. A high value means removing this pixel in a horizontal seam would
create visible artifacts.
Both values are positive numbers (or zero), with higher numbers indicating more important
pixels that should be preserved.
• Viewlmage FUNCTION
public static void ViewlmageRGBPixel[,] ImageMatrix, PictureBox PicBox)
Purpose: This function displays your processed image on screen by converting your internal RGB
pixel format into something Windows can show in a picture box.
INPUT:
Your processed image data - A 2D grid of RGB pixels (the same format created by Openimage or
produced by your seam carving operations)
A PictureBox control - The Windows Forms Ul element where you want the image to appear.
This is like giving the function a "picture frame" to put the image in.
OUTPUT:
The function doesn't return anything (it's void)
• GaussianFilter1D FUNCTION
public static RGBPixel[,] GaussianFilter 1D(
RGBPixel[,] ImageMatrix, int filterSize, double sigma)
Purpose:
This function applies a Gaussian blur to an image to smooth it out and reduce noise. Gaussian
blur is commonly used as a preprocessing step in image processing to eliminate small details
and noise before performing operations like edge detection or seam carving.
Think of it like looking through slightly frosted glass - sharp edges become softer, small spots
disappear, but the overall shape remains recognizable.
INPUT:
You need to give this function three things:
The image data - A 2D grid of colored pixels that makes up your original image
The filter size - A whole number that determines how much blurring happens (bigger number =
more blur). This controls how many neighboring pixels are considered when blurring each pixel.
Sigma (a) - A decimal number that controls the "spread" of the blur. A higher sigma makes the
blur more widespread and gentle, while a lower sigma keeps it more localized.
OUTPUT:
The function returns a new blurred version of your original image with the same dimensions
(same width and height). Al the sharp edges are softened, noise is reduced, but the overall
content remains the same.
CalculateGradientAtPixel FUNCTION
private static Vector2D CalculateGradientAtPixel(
int x, int y, RGBPixel[,] ImageMatrix)
Purpose: This function calculates the intensity gradient (slope or change) at a specific pixel in an
image. The gradient tells us how quickly the image brightness changes at that point and in
which direction it changes fastest. This is a fundamental building block for edge detection,
which is essential for seam carving and many other image processing tasks.
INPUT:
You need to give this function three pieces of information:
The x-coordinate - Which column the pixel is in, counting from the left edge (0 is the leftmost
column)
The y-coordinate - Which row the pixel is in, counting from the top edge (0 is the top row)
The image data - The complete 2D grid of colored pixels that makes up your image
OUTPUT:
The function returns a gradient vector that contains two numbers:
The X component - How much the image brightness changes as you move horizontally (from left
to right) at this pixel. A positive value means it gets brighter going right, negative means it gets
darker.
The Y component - How much the image brightness changes as you move vertically (from top to
bottom) at this pixel. A positive value means it gets brighter going down, negative means it gets
darker going down.
• CLASS LiveWireProcessor
Purpose: This class implements the Live Wire / Intelligent Scissors algorithm for interactive
image segmentation, finds the optimal path between clicks by following image edges.
Key Components Shown:
1. Constants
private const int MAX_RADIUS = 400;
Purpose: Safety limit for search radius to prevent infinite loops or memory issues
Why 400?: Large enough for most practical objects, small enough to be efficient
Adjustable: Can be changed based on image size or application needs
2. Memory Buffers (Reusable)
These buffers store algorithm state and are reused across multiple calls to avoid expensive
memory allocations:
private static double,[] distBuf; // Distance from start to each pixel
private static Point[] parentBuf; // Previous pixel in optimal path
private static bool,[] visitedBuf; // Whether pixel has been processed
private static int bufH = -1, bufW = -1; // Current buffer dimensions
Memory Reuse Pattern:
First call: Allocate buffers
Subsequent calls: Reuse if same size, reallocate if different size
This avoids costly new operations during interactive use
3. Node Structure (Priority Queue Element)
private struct Node
public int x, y; public double d; // Pixel coordinates
// Distance/cost to this pixel
public Node(int x, int y, double d)
this.x = x; this.y = y; this.d = d;
Purpose: Used in the priority queue (like a min-heap) for Dijkstra's algorithm. The queue
always processes the pixel with the smallest distance next.
• Algorithm Steps:
Cost Map: Convert image to edge costs (low cost along edges)
Dijkstra: Compute shortest paths from start point to all pixels
Path Extraction: Trace back from current position to start
Display: Show the optimal path to user
• CLASS MinHeap
Purpose:
This is a minimum heap (priority queue) implementation specifically designed for Dijkstra's
algorithm in the Live Wire processor, ensure the algorithm finds the shortest path efficiently.
Key Components:
1. Core Storage
private List<Node> h = new List<Node>);
Uses a List<Node> for dynamic resizing
Index-based navigation (no pointers needed)
h O = r o o t = s m a l l e s t e l e m e n t
2. Public Interface
public int Count => h.Count; // How many elements
public void Push(Node n); // Add new element
public Node Pop(): // Remove and return smallest
Even though the code uses a loop, it implements a recursive algorithm iteratively
private void Heapify(int i)
while (true)
int | = 2* i + 1, r= 2 * i + 2, s = i;
if (1 < h.Count && h l . d < h i s . d s = 1;
if (r < h.Count && h[r).d < h[s].d) s = r;
if (s == i) break;
Swap(i, s);
i=5;
private void Swap(int a, int b)
1
Node t = h[a);
h[a] = h[b];
h[b] =t;
Purpose:
This function restores the min-heap property starting from a given node that might be
violating it. it ensures that parent nodes are always smaller than their children, which is the
fundamental rule of a min-heap
Preprocessimage FUNCTION
public static RGBPixelL,] PreprocessimageRGBPixel[,) image)
Purpose: This is a convenience function that prepares an image for seam carving by applying a
standardized amount of blurring. Think of it like putting on slightly smudged glasses before
trying to find the least noticeable lines to remove from a picture - the blurring helps the
algorithm focus on important edges without getting distracted by tiny details and noise.
INPUT:
give this function just one thing: your original image as a 2D grid of colored pixels
OUTPUT:
The function returns a new preprocessed version of your image that has been slightly blurred
and smoothed
ComputeEnergy Map FUNCTION
public static Vector2D[,) ComputeEnergyMap(RGBPixel[,) image)
Purpose: This function is meant to calculate an energy map for an entire image. An energy map
is like a "importance heatmap" where every pixel gets a score showing how crucial it is to keep.
High energy pixels are on edges and details that should be preserved; low energy pixels are in
smooth areas that could be removed.
INPUT:
image as a 2D grid of colored pixels
OUTPUT:
A 2D grid (same size as your image) of energy vectors, where each vector has:
• X component: How bad it would be to remove this pixel in a vertical seam
• Y component: How bad it would be to remove this pixel in a horizontal seam
BuildCostGraph FUNCTION
public static double[,] BuildCostGraph(Vector2Dl,) energyMap)
Purpose: This function converts an energy map into a cost graph for dynamic programming in
seam carving. Think of it like converting a "importance map" into actual "cutting prices" - it flips
the values so that high energy (important pixels) becomes low cost (cheap to cut through)
INPUT:
An energy map - a 2D grid where each cell contains a Vector2D with:
• X: Energy for vertical seams
• Y: Energy for horizontal seams
OUTPUT:
A cost graph - a 2D grid of decimal numbers where:
• Low numbers = "Cheap" to cut here (pixels we could remove)
• High numbers = "Expensive" to cut here (pixels we should keep)
• ComputeShortestPaths FUNCTION
public static void ComputeShortestPaths
Purpose: This function implements Dijkstra's algorithm to compute the shortest paths from a
single starting point (anchor) to all other pixels within a search radius. It's the core pathfinding
engine for the Live Wire tool, calculating optimal paths that follow image edges with minimal
c o s t .
• Cost Map (cost) - A 2D grid where each pixel has a number representing how
"expensive" it is to travel through that pixel. Low numbers mean "good paths" (edges),
high numbers mean "bad paths" (flat areas.
• Anchor Coordinates (anchorX, anchorY) - The starting point for all path calculations.
This is typically where the user clicked in the image. Think of it as the "home base" for all
pathfinding.
• Output Parameters (dist, parent) - These are out parameters where the results will be
stored. You provide empty variables, and the function fills them with data.
OUTPUT:
• Distance Map (dist) - A 2D grid where each pixel contains the minimum total cost to
reach that pixel from the anchor point. If a pixel has value 1000, it costs 1000 "units" to
travel there from the start along the optimal path.
• Parent Map (parent) - A 2D grid where each pixel points to its previous pixel in the
optimal path back to the anchor. This creates a chain you can follow backward to
reconstruct the actual path.
• Backtrack FUNCTION
public static List<Point> Backtrack(Point,] parent, int x, int y)
Purpose: This function reconstructs the optimal path from any pixel back to the starting point
(anchor) by following the chain of parent pointers. It's what actually draws the Live Wire on
screen as the user moves their mouse.
INPUT:
• Parent Map (parent) - A 2D grid where each cell points to the "previous pixel" in the
optimal path back to the anchor. This was created by ComputeShortestPaths.
• Current X Coordinate (x) - The column position of the pixel you want to start
backtracking FROM. This is typically where the user's mouse cursor is currently located.
• Current Y Coordinate (y) - The row position of the pixel you want to start backtracking
FROM.
OUTPUT:
The function returns a List of Points that represents the complete path from your starting pixel
back to the anchor. The list is ordered from your starting point TO the anchor (not including the
anchor itself).
DrawPathOptimized FUNCTION:
public static void DrawPathOptimized(RGBPixel[,] img, List<Point> path, byte r, byte g, byte b)
Purpose:
This function visually highlights a path on an image by coloring the pixels along that path with a
specified color. It's the component that shows users where the live Wire path is located as they
interact with the image.
INPUT:
Image Data (img) - The actual pixel data of the image you want to draw on. This is modified
directly (not a copy!), so the path becomes part of the image.
Path to Draw (path) - A list of points representing the sequence of pixels that make up the path.
This typically comes from the Backtrack function and shows the optimal route from the current
mouse position back to the anchor.
Color Components (r, g, b) - The exact color to use for drawing the path, specified as separate
red, green, and blue values (0-255 each).
OUTPUT:
The function doesn't return anything (it's void)
DrawPath FUNCTION
public static void DrawPath(RGBPixel[,] img, List<Point> path)
Purpose:
This function is a convenience wrapper that draws a path on an image using a standardized
bright red color. It's the "default" or "quick-draw" version of the more flexible
DrawPathOptimized function
INPUT:
Image Data (img) - The actual pixel data of the image you want to draw on. This will be modified
directly with red pixels along the path.
Path to Draw (path) - A list of points representing the sequence of pixels that make up the path
you want to visualize.
That's it! No color choices, no extra parameters.
OUTPUT:
The function doesn't return anything, but it modifies your image by drawing the path in bright
red (RGB: 255, 0, 0). The pixels along the path become red.
ViewlmageSafe Function
Purpose:
• This function safely displays an image in a PictureBox while preventing memory leaks by
properly disposing of the previous image.
INPUT:
• Image Data (ImageMatrix) - The 2D grid of RGB pixels you want to display, in the same
format created by Openimage or your image processing functions.
• PictureBox Control (PicBox) - The Windows Forms Ul element where you want the
image to appear. This is your "digital picture frame."
OUTPUT:
The function doesn't return anything, but it has two important effects:
: Displays your new image in the PictureBox (lust like Viewimage)
Safely disposes of the old image that was previously in the PictureBox (if it exists and
isn't being reused)
MAIN FUNCTION
Purpose:
This section of the code declares the core variables used to store image data, track user
interaction, and support energy-based path calculations for intelligent selection.
Input:
This code does not directly receive input in the form of parameters or function arguments.
However, it relies on the following indirect inputs during runtime:
User mouse actions (movement, clicks) to control the lasso tool
• Loaded image data, stored as a 2D array of RGPixel
Anchor point coordinates selected by the user
• Energy values calculated from the image for pathfinding
Output:
This code itself does not generate output directly.
Instead, it prepares and maintains internal data structures that enable:
Dynamic visual feedback while drawing a lasso on the image
Display of the selected or processed image
Computation of optimal paths for intelligent image selection
• Updates to the user interface based on mouse movement and anchor placement
public mainform const.
Purpose:
The MainForm constructor is responsible for initializing the main application window, setting up
the user interface components, and registering mouse event handlers required for interactive
image processing.
It prepares the form to respond to user actions such as clicking and moving the mouse over the
image area.
input:
This constructor does not take any input parameters.
However, during execution it relies on:
Predefined Ul components created by the Windows Forms designer
User mouse interactions on the image display area (mainPictureBox)
Output:
The constructor does not return a value.
Its effect is to produce a fully initialized and interactive main form, ready for user input, by:
• Displaying the main window
• Hiding unnecessary Ul indicators at startup
• Enabling mouse interaction on the image area
Method: menuButton_Click
Purpose:
The menuButton_Click method handles menu button click events and provides visual feedback
to the user by moving and displaying an indicator panel.
Its purpose is to highlight the currently selected menu option and improve user interface
usability.
Input:
object sender
The menu button that triggered the click event.
EventArgs e
Event data associated with the click action (not directly used in this method).
Output:
This method does not return a value.
Its output is a visual change in the user interface, including:
• Repositioning the indicator panel to align with the clicked button
• Resizing the indicator panel to match the button's height
• Changing the background color of the selected button
• Displaying the indicator panel to show the active menu item
Method: menuButton_Leave
Purpose:
The menuButton_Leave method restores the default appearance of a menu button when the
mouse pointer leaves it.
Its purpose is to maintain consistent visual behavior and remove the active or hover highlight
when the button is no longer in focus.
Input:
object sender
The menu button control that triggered the event.
EventArgs e
Event data related to the mouse leave action (not directly used).
Output:
This method does not return a value.
Its output is a visual Ul update, specifically resetting the menu button's background color to its
original state.
Method: exitToolStripMenultem_Click
Purpose:
It handles the Exit menu option and safely terminates the application.
Input: object sender
The menu item that triggered the exit command.
EventArgs e
Event data associated with the click event (not directly used).
Output:
This method does not return a value.
Its output is the termination of the application, including:
• Closing all open forms
• Releasing system resources
Method: openToolStripMenultem_Click
Purpose:
allows the user to load an image into the application, display it in the main picture box, and
prepare it for interactive processing.
It also initializes the internal data structures used for image manipulation, energy map
computation, and pathfinding for the lasso tool.
Input:
object sender
The menu item that triggered the event (Open).
EventArgs e
Event data associated with the click event (not directly used).
User interaction:
The method also relies on user selection from the OpenFileDialog, which provides the path to
the image file.
Output:
This method does not return a value, but it produces several effects:
• Loads the selected image into ImageMatrix.
• Displays the image in mainPictureBox.
• Initializes copies of the image for processing:
• fixedimage for permanent changes
• tempDisplayimage for temporary display during interaction
• Processes the image to generate energy maps and cost graphs for the LiveWire
(intelligent lasso) algorithm:
• EnergyMap
• CostGraph
• Updates Ul elements (txtWidth, txtHeight) to show the image dimensions.
• Resets anchor point and mouse tracking variables for a fresh selection.
Method: clearToolStripMenultem_Click
Purpose:
points.
resets the current image view to its original state and clears any active selection or anchor
It allows the user to undo temporary changes made during interactive image processing.
Input:
object sender
The menu item that triggered the event (Clear).
EventArgs e
Event data associated with the click event (not directly used).
Internal dependency: Requires fixedImage to be initialized (i.e., an image must be loaded).
Output:
This method does not return a value.
Its effects include:
Restoring the mainPictureBox display to the original image (fixedimage).
Resetting anchor-related variables to indicate no active selection:
hasAnchor = false
anchorX = -1, anchorY = -1
Method: binLivewire_Click
Purpose:
activates the LiveWire lasso tool for interactive image selection.
It updates the Ul to indicate the active tool and changes the cursor to provide visual feedback
for the user.
Input:
object sender
The LiveWire button that triggered the event.
EventArgs e
Event data associated with the click (not directly used).
Output:
This method does not return a value.
Its effects include:
• Highlighting the LiveWire button in the menu using menuButton_Click.
• Changing the cursor over the image to a crosshair to indicate selection mode.
• Setting isLassoEnabled = true to enable lasso functionality.
Method: binLivewire_Leave
Purpose:
deactivates the LiveWire tool when the button loses focus or the user moves away.
It restores the cursor and Ul to the default state.
Input:
object sender
The LiveWire button that triggered the event.
EventArgs e
Event data associated with the mouse leave action.
Output:
This method does not return a value.
Its effects include:
• Restoring the menu button appearance using menuButton_Leave.
• Changing the cursor back to the default pointer.
• Setting isLassoEnabled = false to disable lasso interactions.
Method: mainPictureBox_MouseClick
Purpose:
handles mouse click events on the image area for the Live Wire lasso tool.
It alows the user to set anchor points, compute optimal paths, and draw selections interactively
based on the precomputed cost graph.
Input:
object sender
The picture box control (mainPictureBox) where the click occurred.
MouseEventArgs e
Provides the X and Y coordinates of the mouse click relative to the image.
Internal dependencies:
Requires:
• isLassoEnabled = true (LiveWire tool is active)
• CostGraph to be computec
• fixedimage to be loadec
Output:
This method does not return a value.
Its effects include:
• Setting an anchor point if none exists, or using the current anchor to compute a
path.
• Computing shortest paths from the anchor using the cost graph.
• Backtracking the path from the anchor to the clicked point.
• Drawing the selected path on the image.
• Updating the displayed image in mainPictureBox.
• Updating anchor and anchorY for continued lasso selection.
• Copying the updated image to tempDisplay|mage for temporary display during
further interactions.
• Resetting lastMouseX and lastMouseY to indicate a fresh starting point for mouse
m o v e m e n t .
Method: mainPictureBox_MouseMove
Purpose:
tracks mouse movement over the image while the Live Wire lasso tool is active.
It provides real-time visual feedback by dynamicaly displaying the shortest path from the
current anchor point to the mouse pointer, enabling precise image selection.
Input:
object sender
The picture box control (mainPictureBox) where the mouse is moving.
MouseEventArgs e
Provides the X and Y coordinates of the mouse pointer relative to the image.
Internal dependencies: Requires:
: hasent arra-true an anchor is already set) Parent array from previously computed shortest paths
• fixedimage to be loaded
Output:
This method does not return a value.
Its effects include:
• Updating the mouse position display in the UI (txtMousePosX, txtMousePosY).
• Skipping updates if the mouse movement is below a defined threshold
(MOUSE_MOVE_THRESHOLD) to optimize performance.
• Copying fixedlmage into tempDisplaylmage for temporary display.
• Dynamically computing the path from the anchor to the current mouse position
using LiveWireProcessor.Backtrack.
• Drawing the computed path in real-time using
LiveWireProcessor.DrawPathOptimized (highlighted in yellow).
• Updating mainPictureBox to reflect the live path without altering the original image
Method: menuStrip1_ItemClicked
Purpose:
an event handler placeholder for handling click events on items in the menu strip.
Input:
object sender
The menu strip control (menuStrip1) where the item was clicked.
ToolStripItemClickedEventArgs e
Provides information about the clicked menu item (e.g., which item was clicked).
Output:
This method does not return a value and currently does not produce any output.
