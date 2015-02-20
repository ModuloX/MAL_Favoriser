using MarkusRezai.Project.UltraGraph.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Petzold.Media2D;
using MarkusRezai.Project.UltraGraph.ViewModel;
using Microsoft.Win32;
using MahApps.Metro.Controls;
using System.Globalization;

namespace MarkusRezai.Project.UltraGraph.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Scrollviewer drag and zoom logic by Kevin Stumpf: http://www.codeproject.com/Articles/97871/WPF-simple-zoom-and-drag-support-in-a-ScrollViewer
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Classwide Properties, Fields and Constants

        /// <summary>
        /// Indicates the offset of the logical origin point on the <see cref="MainWindow.MainCanvas"/>.
        /// </summary>
        public Vector OriginRealOffset { get { return new Vector(MainCanvas.Width / 2, MainCanvas.Height / 2); } }

        /// <summary>
        /// The displayed <see cref="Graph"/>. (Semantic) content of the <see cref="MainWindow.MainCanvas"/>.
        /// </summary>
        public Graph MainGraph { get; set; }
        /// <summary>
        /// Indicates, whether the <see cref="MainWindow.MainGraph"/> changed since the last save.
        /// </summary>
        public bool UnsavedProgress { get; set; }

        #endregion

        #region Constructor and Initializers

        public MainWindow()
        {
            InitializeComponent();

            // Add events for scrollviewer drag and zoom to scrollviewer and slider
            // Drag events are set to preview so that the drag operation starts and ends before other events check for it
            MainScrollViewer.PreviewMouseRightButtonDown += ScrollViewer_OnPreviewMouseRightButtonDown_StartDrag;
            MainScrollViewer.PreviewMouseRightButtonUp += ScrollViewer_OnPreviewMouseRightButtonUp_EndDrag;
            MainScrollViewer.PreviewMouseMove += ScrollViewer_OnPreviewMouseMove_Drag;

            MainScrollViewer.ScrollChanged += ScrollViewer_OnScrollChanged;
            MainScrollViewer.PreviewMouseWheel += ScrollViewer_OnPreviewMouseWheel;

            MainSlider.ValueChanged += Slider_OnValueChanged;

            // Add events to canvas
            MainCanvas.MouseLeftButtonUp += canvas_ToolModeNewVertex;

            // Drag events, the end drag is here to ensure that the drag operation is ended even when the mouse is outside the visible canvas
            MainCanvas.PreviewMouseLeftButtonUp += ellipse_EndDrag;
            MainCanvas.PreviewMouseMove += canvas_ellipse_Drag;

            // Add a delegate to the MainScrollViewer to scroll to the origin as soon as the main canvas is loaded and therefore its actual height and width are accessible
            MainCanvas.Loaded += delegate { ScrollToOrigin(); };

            Initialize();

            LoadGraph(DEBUG_CREATE_TEST_GRAPH());
        }

        /// <summary>
        /// Initializes class members.
        /// </summary>
        private void Initialize()
        {
            UnsavedProgress = false;

            CurrentToolMode = ToolMode.None;
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Saves the <see cref="Graph"/> in form of XML data. Displays a <see cref="SaveFileDialog"/> to get the file name.
        /// </summary>
        /// <returns>True if the <see cref="Graph"/> was saved successfully.</returns>
        private bool Save()
        {
            XComment comment = new XComment("This is automatically generated XML code, representing a Graph. Do not change anything, unless you absolutely know what you are doing - you may corrupt and break this file.");
            XElement content = new XElement("Graph",
                                            MainGraph.ToXElement(),
                                            ViewDataManager.ToXElement(),
                                            DetailDataTemplateManager.ToXElement());

            XDocument xDocument = new XDocument(comment, content);

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "Graph";
            dialog.DefaultExt = ".ugraph";
            dialog.Filter = "Ultra Graph|*.ugraph|XML Document|*.xml|Text Document|*.txt|All Files|*.*";
            dialog.AddExtension = true;
            dialog.OverwritePrompt = true;

            if (dialog.ShowDialog() == true)
            {
                xDocument.Save(dialog.FileName);
                UnsavedProgress = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles unsaved progress: Displays a <see cref="SaveFileDialog"/> asking the user if they want to save and calls <see cref="MainWindow.Save()"/> if the agree.
        /// <returns>True if unsaved progress was handled successfully.</returns>
        /// </summary>
        private bool HandleUnsavedProgress()
        {
            MessageBoxResult result = MessageBox.Show("Do you want to save your progress before quitting?", "Unsaved Progress", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);

            // If the user wants to save and the save operation was successful return true
            if (result == MessageBoxResult.Yes && MainGraph != null) return Save();

            // If the user
            return result == MessageBoxResult.No;
        }

        private void Load(XDocument xSource)
        {
            // First load all resources
            ViewDataManager.LoadViewData(xSource.Root.Element("ViewDataDictionary"));
            DetailDataTemplateManager.LoadDetailDataTepmlates(xSource.Root.Element("DetailDataTemplateDictionary"));

            // Lastly load Graph, which uses previously loaded resources
            LoadGraph(new Graph(xSource.Root.Element("GraphData")));
        }

        /// <summary>
        /// Loads a <see cref="Graph"/> into the <see cref="MainWindow.MainCanvas"/>.
        /// </summary>
        /// <param name="graph">The <see cref="Graph"/> to be loaded.</param>
        private void LoadGraph(Graph graph)
        {
            MainGraph = graph;
            UnsavedProgress = false;

            ScrollToOrigin();

            DrawCanvas(true);
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Used for performance purposes, represents <see cref="Math.PI"/> / 2
        /// </summary>
        private static readonly double PiHalf = Math.PI / 2;
        /// <summary>
        /// Used for performance purposes, represents <see cref="Math.PI"/> * 3 / 2
        /// </summary>
        private static readonly double PiThreeHalf = Math.PI * 3 / 2;

        /// <summary>
        /// Draws the <see cref="MainWindow.MainCanvas"/> and its contents. It draws the <see cref="MainWindow.MainGraph"/>, so it has to be set.
        /// </summary>
        /// <param name="redraw">Indicates whether the <see cref="MainWindow.MainCanvas"/> will be cleared before the draw operation.</param>
        private void DrawCanvas(bool redraw)
        {
            if (redraw) MainCanvas.Children.Clear();

            if (MainGraph == null) throw new InvalidOperationException("ContentGraph is null.");

            // Set the size of the canvas, according to the graph information
            MainCanvas.Width = MainGraph.CanvasSize.Width;
            MainCanvas.Height = MainGraph.CanvasSize.Height;

            DrawEdgeArrowLines(MainGraph.Edges);
            DrawVertexEllipses(MainGraph.Vertices);
        }

        /// <summary>
        /// Adds a <see cref="Vertex"/> as <see cref="Ellipse"/> to the <see cref="MainWindow.MainCanvas"/>. Sets and its Name and draws it. The <see cref="Vertex"/> will be set as <see cref="Ellipse.DataContext"/> for later operations.
        /// </summary>
        /// <param name="vertex">The <see cref="Vertex"/> that will be drawn.</param>
        private void DrawVertexEllipse(Vertex vertex)
        {
            // Register and set their name to CIV followed by their ComponentId, eg. "CIV102"
            Ellipse ellipse = new Ellipse();
            ellipse.Name = "CIV" + vertex.ComponentId;

            // Set the edge object as DataContext for later retrieval
            ellipse.DataContext = vertex;

            // Draw a formatted text using an image control at the right position with a ZIndex of 200, which should be higher than any other visual element on the canvas
            drawVertexName(vertex.Name, vertex.Radius, vertex.ViewData.FillBrush.Color == Colors.Black, vertex.Position);
            if (vertex.Name != string.Empty) ellipse.ToolTip = vertex.Name;

            // Add events for tool modes, drag and detail data
            ellipse.MouseLeftButtonUp += ellipse_ToolModeConnectNoneSelected;
            ellipse.MouseLeftButtonUp += ellipse_ToolModeConnectVertexSelected;
            ellipse.MouseLeftButtonUp += ellipse_ToolModeEdit;
            ellipse.MouseLeftButtonUp += ellipse_ToolModeDelete;
            ellipse.MouseLeftButtonUp += ellipse_ToolModeReconnectEdgeSelected;
            ellipse.MouseLeftButtonUp += ellipse_ToolModeReconnectEdgeAndVertexSelected;

            // Must be preview to prevent a drag operation from starting when double clicking
            ellipse.PreviewMouseLeftButtonDown += ellipse_ShowDetailData;

            // Drag events 
            ellipse.MouseLeftButtonDown += ellipse_StartDrag;
            ellipse.PreviewMouseLeftButtonUp += ellipse_EndDrag;

            // Set view data
            ViewDataManager.ApplyVertexEllipseViewData(vertex, ellipse);

            // Set the radius
            ellipse.Height = vertex.Radius * 2;
            ellipse.Width = vertex.Radius * 2;

            // Position the ellipse on the canvas, since one can only set the distance from the canvas top, the Y coordinate has to be subtracted from the origin
            ellipse.SetValue(Canvas.LeftProperty, OriginRealOffset.X + vertex.Position.X - ellipse.Width / 2);
            ellipse.SetValue(Canvas.TopProperty, OriginRealOffset.Y - vertex.Position.Y - ellipse.Height / 2);

            // Set the ZIndex
            ellipse.SetValue(Canvas.ZIndexProperty, vertex.ZIndex);

            MainCanvas.Children.Add(ellipse);
        }

        /// <summary>
        /// Adds an <see cref="Edge"/> as <see cref="ArrowLine"/> to the <see cref="MainWindow.MainCanvas"/>. Sets and its Name and draws it. The <see cref="Edge"/> will be set as <see cref="ArrowLine.DataContext"/> for later operations.
        /// </summary>
        /// <param name="edge">The <see cref="Edge"/> that will be drawn.</param>
        private void DrawEdgeArrowLine(Edge edge)
        {
            // Register and set their name to CIE followed by their ComponentId, eg. "CIE102"
            ArrowLine arrowLine = new ArrowLine();
            arrowLine.Name = "CIE" + edge.ComponentId;

            // Set the edge object as DataContext for later retrieval
            arrowLine.DataContext = edge;

            // Add mousedown events
            arrowLine.MouseLeftButtonUp += arrowLine_ToolModeReconnectNoneSelected;
            arrowLine.MouseLeftButtonUp += arrowLine_ToolModeEdit;
            arrowLine.MouseLeftButtonUp += arrowLine_ToolModeDelete;

            // Must be preview to prevent other mousedowns from happening when double clicking
            arrowLine.PreviewMouseLeftButtonDown += arrowLine_ShowDetailData;

            // Set view data
            ViewDataManager.ApplyEdgeArrowLineViewData(edge, arrowLine);

            // Check if edge is directed and disable the arrow end if it's not
            if (!edge.IsDirected) arrowLine.ArrowEnds = ArrowEnds.None;

            // Position the arrowLine on the canvas

            // The arrowLines are only set to the origin here, the remaining positioning is done through the X1/Y1 and X2/Y2 Attributes of the arrowline
            arrowLine.SetValue(Canvas.LeftProperty, OriginRealOffset.X);
            arrowLine.SetValue(Canvas.TopProperty, OriginRealOffset.Y);

            // Set the X and Y coordinates of the head and tail
            // Since we need the intersections of the edge and the vertex circle, this requires some magic (maths)
            // First make locals for the Positions of Vertex A and B, the radii of their ellipses and the distance vector between them
            Vector A = edge.VertexA.Position;
            Vector B = edge.VertexB.Position;

            double radiusA = edge.VertexA.Radius;
            double radiusB = edge.VertexB.Radius;

            // The distance has to be negated for beta, since we look at the situation from the opposite angle
            // The calculaion would be distance = A - B, instead of distance = B - A, but this is more performant
            Vector distanceA = B - A;
            Vector distanceB = A - B;

            // Calculate relative angles of the arrowLine, where alpha is on A and beta is on B
            // This is where shit gets real

            // The initial value is set to Pi / 2, later it could be set to Pi * 3 / 2
            // This is for the two cases, where the tangent cannot be applied, due to distance.X or distance.Y being 0
            // This is the case, when both vertex-centers are on a horizontal or vertical line

            // Another case differentiation is necessary due to the ambiguity of the tangent's result:
            // The tangent of angles of the first and third or second and fourth quadrant (when translating the angle to a Cartesian coordinate) 
            // result in the same value so the arc tangent in itself can't uniquely determine the angle, meaning that, without any corrections, 
            // only angles of the first and fourth quadrant will be displayed (tan results only range from -90° to 90°).

            double alpha = PiHalf;

            if (distanceA.X < 0) alpha = Math.Atan(distanceA.Y / distanceA.X) + Math.PI;
            else if (distanceA.X > 0) alpha = Math.Atan(distanceA.Y / distanceA.X);
            else if (distanceA.Y < 0) alpha = PiThreeHalf;

            double beta = PiHalf;

            if (distanceB.X < 0) beta = Math.Atan(distanceB.Y / distanceB.X) + Math.PI;
            else if (distanceB.X > 0) beta = Math.Atan(distanceB.Y / distanceB.X);
            else if (distanceB.Y < 0) beta = PiThreeHalf;

            // Calculate offsets
            Vector offsetA = new Vector(radiusA * Math.Cos(alpha), radiusA * Math.Sin(alpha));
            Vector offsetB = new Vector(radiusB * Math.Cos(beta), radiusB * Math.Sin(beta));

            Vector newPositionA = new Vector(edge.VertexA.Position.X + offsetA.X, edge.VertexA.Position.Y + offsetA.Y);
            Vector newPositionB = new Vector(edge.VertexB.Position.X + offsetB.X, edge.VertexB.Position.Y + offsetB.Y);

            // Apply the offsets to the positions of the Vertices, which results in the relative arrowLine head and tail position to the origin
            // Due to the inverted Y-Axis (courtesy of the canvas) the Y values are negated
            arrowLine.X1 = newPositionA.X;
            arrowLine.Y1 = -newPositionA.Y;

            arrowLine.X2 = newPositionB.X;
            arrowLine.Y2 = -newPositionB.Y;

            // Draw edge name
            string text = edge.Name;

            if (edge.IsWeighted)
            {
                if (text != string.Empty) text += " - ";
                text += edge.WeightDescriptor + ": " + edge.WeightValue + " " + edge.WeightUnit;
            }

            drawEdgeName(text, (newPositionA - newPositionB).Length, newPositionA + (newPositionB - newPositionA) / 2);
            if (text != string.Empty) arrowLine.ToolTip = text;

            // Set ZIndex
            arrowLine.SetValue(Canvas.ZIndexProperty, edge.ZIndex);

            MainCanvas.Children.Add(arrowLine);
        }

        /// <summary>
        /// Adds all <see cref="Vertex"/>es as <see cref="Ellipse"/>s to the <see cref="MainWindow.MainCanvas"/>. Sets their Names and draws them.
        /// </summary>
        /// <param name="vertices">A list of all <see cref="Vertex"/>es that will be drawn.</param>
        private void DrawVertexEllipses(List<Vertex> vertices)
        {
            foreach (Vertex vertex in vertices)
            {
                DrawVertexEllipse(vertex);
            }
        }

        /// <summary>
        /// Adds all <see cref="Edge"/> as <see cref="ArrowLine"/>s to the <see cref="MainWindow.MainCanvas"/>. Sets their Names and draws them.
        /// </summary>
        /// <param name="edges">A list of all <see cref="Edge"/>s that will be drawn.</param>
        private void DrawEdgeArrowLines(List<Edge> edges)
        {
            foreach (Edge edge in edges)
            {
                DrawEdgeArrowLine(edge);
            }
        }

        /// <summary>
        /// Draws the name of a <see cref="Vertex"/>. The size of the text block is scaled according to the main scale transform.
        /// </summary>
        /// <param name="name">The name that will be drawn.</param>
        /// <param name="radius">The radius of the <see cref="Vertex"/>. This is used to shrink the text down in case it is too long to fit inside the <see cref="Vertex"/>.</param>
        /// <param name="outline">Indicates whether the name will be drawn with a white text outline.</param>
        /// <param name="position">The position of the <see cref="Vertex"/>.</param>
        private void drawVertexName(string name, double radius, bool outline, Vector position)
        {
            // Create a formatted text from which geometry data can be procured
            FormattedText formattedText = new FormattedText(name,
                                                            CultureInfo.GetCultureInfo("en-us"),
                                                            FlowDirection.LeftToRight,
                                                            new Typeface(new FontFamily("Arial"), FontStyles.Normal, outline ? FontWeights.Bold : FontWeights.Normal, FontStretches.Normal),
                                                            30 / MainScaleTransform.ScaleX,
                                                            Brushes.Black);

            // Create a drawing group for creating a drawing context which is later used as image source
            DrawingGroup dGroup = new DrawingGroup();

            // Add the geometry of the formatted text to the drawing group drawing context
            // If the text is drawn with a white outline, then a pen is used
            if (outline) using (DrawingContext dc = dGroup.Open()) dc.DrawGeometry(Brushes.Black, new Pen(Brushes.White, 0.7 / MainScaleTransform.ScaleX), formattedText.BuildGeometry(new Point(0, 0)));
            else using (DrawingContext dc = dGroup.Open()) dc.DrawGeometry(Brushes.Black, null, formattedText.BuildGeometry(new Point(0, 0)));

            // Create an image with a drawing image as source which uses the drawing group as its content
            DrawingImage dImage = new DrawingImage(dGroup);
            Image image = new Image() { Source = dImage };

            // Check if the drawing image is too large for the vertex
            double maxWidth = radius * 1.8;
            if (dImage.Width > maxWidth)
            {
                // Calculate the ratio used to shrink the text size
                double ratio = maxWidth / dImage.Width;

                // Recreate the formatted text and update the drawing group drawin context
                formattedText.SetFontSize(30 / MainScaleTransform.ScaleX * ratio);
                if (outline) using (DrawingContext dc = dGroup.Open()) dc.DrawGeometry(Brushes.Black, new Pen(Brushes.White, 0.7 / MainScaleTransform.ScaleX * ratio), formattedText.BuildGeometry(new Point(0, 0)));
                else using (DrawingContext dc = dGroup.Open()) dc.DrawGeometry(Brushes.Black, null, formattedText.BuildGeometry(new Point(0, 0)));
            }

            // Place the image on the canvas
            image.SetValue(Canvas.LeftProperty, OriginRealOffset.X + position.X - dImage.Width / 2);
            image.SetValue(Canvas.TopProperty, OriginRealOffset.Y - position.Y - dImage.Height / 2);
            image.SetValue(Canvas.ZIndexProperty, 200);

            MainCanvas.Children.Add(image);
        }

        /// <summary>
        /// Draws the name of a <see cref="Edge"/>. The size of the text block is scaled according to the main scale transform.
        /// </summary>
        /// <param name="name">The name that will be drawn.</param>
        /// <param name="position">The position of the <see cref="Edge"/>.</param>
        private void drawEdgeName(string name, double length, Vector position)
        {
            // Create a formatted text from which geometry data can be procured
            FormattedText formattedText = new FormattedText(name,
                                                            CultureInfo.GetCultureInfo("en-us"),
                                                            FlowDirection.LeftToRight,
                                                            new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                                                            30 / MainScaleTransform.ScaleX,
                                                            Brushes.Black);

            // Create a drawing group for creating a drawing context which is later used as image source
            DrawingGroup dGroup = new DrawingGroup();

            // Add the geometry of the formatted text to the drawing group drawing context
            using (DrawingContext dc = dGroup.Open())
            {
                dc.DrawGeometry(Brushes.White, new Pen(Brushes.Black, 1 / MainScaleTransform.ScaleX), formattedText.BuildHighlightGeometry(new Point(0, 0)));
                dc.DrawGeometry(Brushes.Black, null, formattedText.BuildGeometry(new Point(0, -0.5 / MainScaleTransform.ScaleX)));
            }

            // Create an image with a drawing image as source which uses the drawing group as its content
            DrawingImage dImage = new DrawingImage(dGroup);
            Image image = new Image() { Source = dImage };

            //// Check if the drawing image is too large for the arrowLine
            //double maxWidth = length * 0.8;
            //if (dImage.Width > maxWidth)
            //{
            //    // Calculate the ratio used to shrink the text size
            //    double ratio = maxWidth / dImage.Width;

            //    // Recreate the formatted text and update the drawing group drawin context
            //    formattedText.SetFontSize(20 * ratio);
            //    using (DrawingContext dc = dGroup.Open())
            //    {
            //        dc.DrawGeometry(Brushes.White, new Pen(Brushes.Black, 1 * ratio), formattedText.BuildHighlightGeometry(new Point(0, 0)));
            //        dc.DrawGeometry(Brushes.Black, null, formattedText.BuildGeometry(new Point(0, -0.5 * ratio)));
            //    }
            //}

            // Place the image on the canvas
            image.SetValue(Canvas.LeftProperty, OriginRealOffset.X + position.X - dImage.Width / 2);
            image.SetValue(Canvas.TopProperty, OriginRealOffset.Y - position.Y - dImage.Height / 2);
            image.SetValue(Canvas.ZIndexProperty, 200);

            MainCanvas.Children.Add(image);
        }

        /// <summary>
        /// Scrolls the <see cref="MainWindow.MainScrollViewer"/> to the origin of the canvas.
        /// </summary>
        public void ScrollToOrigin()
        {
            double offsetX = OriginRealOffset.X * MainScaleTransform.ScaleX - MainScrollViewer.ActualWidth / 2;
            double offsetY = OriginRealOffset.Y * MainScaleTransform.ScaleY - MainScrollViewer.ActualHeight / 2;

            if (double.IsNaN(offsetX) || double.IsNaN(offsetY)) return;

            MainScrollViewer.ScrollToHorizontalOffset(offsetX);
            MainScrollViewer.ScrollToVerticalOffset(offsetY);
        }

        #endregion

        #region ScrollViewer Drag and Zoom Envents

        private Point? scrollViewerLastCenterPositionOnTarget { get; set; }
        private Point? scrollViewerLastMousePositionOnTarget { get; set; }
        private Point? scrollViewerLastDragPoint { get; set; }

        void ScrollViewer_OnPreviewMouseRightButtonDown_StartDrag(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(MainScrollViewer);
            if (mousePos.X <= MainScrollViewer.ViewportWidth && mousePos.Y <
                MainScrollViewer.ViewportHeight) //make sure we still can use the scrollbars
            {
                MainScrollViewer.Cursor = Cursors.SizeAll;
                scrollViewerLastDragPoint = mousePos;
                Mouse.Capture(MainScrollViewer);
            }
        }

        void ScrollViewer_OnPreviewMouseRightButtonUp_EndDrag(object sender, MouseButtonEventArgs e)
        {
            MainScrollViewer.Cursor = Cursors.Arrow;
            MainScrollViewer.ReleaseMouseCapture();
            scrollViewerLastDragPoint = null;
        }

        void ScrollViewer_OnPreviewMouseMove_Drag(object sender, MouseEventArgs e)
        {
            if (scrollViewerLastDragPoint.HasValue)
            {
                Point posNow = e.GetPosition(MainScrollViewer);

                double dX = posNow.X - scrollViewerLastDragPoint.Value.X;
                double dY = posNow.Y - scrollViewerLastDragPoint.Value.Y;

                scrollViewerLastDragPoint = posNow;

                MainScrollViewer.ScrollToHorizontalOffset(MainScrollViewer.HorizontalOffset - dX);
                MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset - dY);
            }
        }

        void ScrollViewer_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollViewerLastMousePositionOnTarget = Mouse.GetPosition(MainCanvas);

            if (e.Delta > 0)
            {
                MainSlider.Value += 0.2;
            }
            if (e.Delta < 0)
            {
                MainSlider.Value -= 0.2;
            }

            e.Handled = true;
        }

        void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MainScaleTransform.ScaleX = e.NewValue;
            MainScaleTransform.ScaleY = e.NewValue;

            var centerOfViewport = new Point(MainScrollViewer.ViewportWidth / 2,
                                             MainScrollViewer.ViewportHeight / 2);
            scrollViewerLastCenterPositionOnTarget = MainScrollViewer.TranslatePoint(centerOfViewport, MainCanvas);

            DrawCanvas(true);
        }

        void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                if (!scrollViewerLastMousePositionOnTarget.HasValue)
                {
                    if (scrollViewerLastCenterPositionOnTarget.HasValue)
                    {
                        var centerOfViewport = new Point(MainScrollViewer.ViewportWidth / 2,
                                                         MainScrollViewer.ViewportHeight / 2);
                        Point centerOfTargetNow =
                              MainScrollViewer.TranslatePoint(centerOfViewport, MainCanvas);

                        targetBefore = scrollViewerLastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = scrollViewerLastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(MainCanvas);

                    scrollViewerLastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double multiplicatorX = e.ExtentWidth / MainCanvas.Width;
                    double multiplicatorY = e.ExtentHeight / MainCanvas.Height;

                    double newOffsetX = MainScrollViewer.HorizontalOffset -
                                        dXInTargetPixels * multiplicatorX;
                    double newOffsetY = MainScrollViewer.VerticalOffset -
                                        dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }

                    MainScrollViewer.ScrollToHorizontalOffset(newOffsetX);
                    MainScrollViewer.ScrollToVerticalOffset(newOffsetY);
                }
            }
        }

        #endregion

        #region Canvas, Ellipse and ArrowLine Events

        /// <summary>
        /// Indicator for the current <see cref="ToolMode"/>. Used to determine what tool is selected and in which state that operation is.
        /// </summary>
        private ToolMode CurrentToolMode { get; set; }
        /// <summary>
        /// Used to store the first <see cref="Vertex"/> of a Connect or Reconnect operation.
        /// </summary>
        private Vertex SelectedVertexTail { get; set; }
        /// <summary>
        /// Used to store the <see cref="Edge"/> of a Reconnect operation.
        /// </summary>
        private Edge SelectedEdge { get; set; }

        private Ellipse ellipseDragTarget { get; set; }
        private Point? ellipseLastDragPoint { get; set; }

        /// <summary>
        /// Unselects selected tool and clears all tool mode influences.
        /// </summary>
        private void ClearToolModes()
        {
            if (CurrentToolMode == ToolMode.NewVertex)
            {
                MainCanvas.Cursor = Cursors.Arrow;
            }

            else if (CurrentToolMode == ToolMode.ConnectNoneSelected)
            {
                foreach (Ellipse ellipse in MainCanvas.Children.OfType<Ellipse>()) { ellipse.Cursor = Cursors.Arrow; }
            }

            else if (CurrentToolMode == ToolMode.ConnectVertexSelected)
            {
                foreach (Ellipse ellipse in MainCanvas.Children.OfType<Ellipse>()) { ellipse.Cursor = Cursors.Arrow; }
                SelectedVertexTail = null;
            }

            else if (CurrentToolMode == ToolMode.ReconnectNoneSelected)
            {
                foreach (ArrowLine arrowLine in MainCanvas.Children.OfType<ArrowLine>()) { arrowLine.Cursor = Cursors.Arrow; }
            }

            else if (CurrentToolMode == ToolMode.ReconnectEdgeSelected)
            {
                foreach (Ellipse ellipse in MainCanvas.Children.OfType<Ellipse>()) { ellipse.Cursor = Cursors.Arrow; }
                SelectedEdge = null;
            }

            else if (CurrentToolMode == ToolMode.ReconnectEdgeAndVertexSelected)
            {
                foreach (Ellipse ellipse in MainCanvas.Children.OfType<Ellipse>()) { ellipse.Cursor = Cursors.Arrow; }
                SelectedEdge = null;
                SelectedVertexTail = null;
            }

            else if (CurrentToolMode == ToolMode.Edit)
            {
                foreach (Shape shape in MainCanvas.Children.OfType<Shape>()) { shape.Cursor = Cursors.Arrow; }
            }

            else if (CurrentToolMode == ToolMode.Delete)
            {
                foreach (Shape shape in MainCanvas.Children.OfType<Shape>()) { shape.Cursor = Cursors.Arrow; }
            }

            CurrentToolMode = ToolMode.None;
        }

        private void startDrag(Ellipse target, Point startPoint)
        {
            MainCanvas.Cursor = Cursors.Hand;
            ellipseLastDragPoint = startPoint;
            ellipseDragTarget = target;
            Mouse.Capture(MainCanvas, CaptureMode.SubTree);
            ((Vertex)ellipseDragTarget.DataContext).ZIndex = 100;
        }

        private void endDrag()
        {
            MainCanvas.Cursor = Cursors.Arrow;
            MainCanvas.ReleaseMouseCapture();
            ((Vertex)ellipseDragTarget.DataContext).ZIndex = 20;
            ellipseDragTarget = null;
            ellipseLastDragPoint = null;
        }

        /// <summary>
        /// The mousedown event for the <see cref="MainWindow.MainCanvas"/>. Used to add <see cref="Vertex"/>es to the <see cref="Graph"/>.
        /// </summary>
        private void canvas_ToolModeNewVertex(object sender, MouseButtonEventArgs e)
        {
            if (MainGraph == null) return;
            if (CurrentToolMode != ToolMode.NewVertex) return;

            // The mouse position must be defined here, since it somehow gets fucked up when showing the dialog
            Vector clickPosition = new Vector(e.GetPosition(MainCanvas).X, e.GetPosition(MainCanvas).Y);

            // Convert click position to the Cartesian coordinate system
            clickPosition.Y = MainCanvas.Height - clickPosition.Y;
            clickPosition -= OriginRealOffset;

            Vertex result = new Vertex();

            VertexPropertiesWindow dialog = new VertexPropertiesWindow();
            dialog.VertexName = result.Name;
            dialog.Radius = result.Radius;
            dialog.ViewData = result.ViewData;
            dialog.Tags = result.Tags;

            if (dialog.ShowDialog() != true)
            {
                ClearToolModes();
                return;
            }

            result.Name = dialog.VertexName;
            result.Position = clickPosition;
            result.Radius = dialog.Radius;
            result.ViewData = dialog.ViewData;
            result.Tags = dialog.Tags;

            // Copy field lists of detail data template
            if (dialog.DetailData != null)
            {
                result.DetailData.StringFields = new List<DetailDataField<string>>(dialog.DetailData.StringFields);
                result.DetailData.LongFields = new List<DetailDataField<long>>(dialog.DetailData.LongFields);
                result.DetailData.DecimalFields = new List<DetailDataField<decimal>>(dialog.DetailData.DecimalFields);
                result.DetailData.BoolFields = new List<DetailDataField<bool>>(dialog.DetailData.BoolFields);
                result.DetailData.DateFields = new List<DetailDataField<DateTime>>(dialog.DetailData.DateFields);
            }

            MainGraph.AddVertex(result);
            UnsavedProgress = true;

            DrawCanvas(true);

            ClearToolModes();

            // Do that to prevent other canvas events from triggering right after this is finished
            e.Handled = true;
        }

        private void ellipse_ShowDetailData(object sender, MouseButtonEventArgs e)
        {
            if (CurrentToolMode != ToolMode.None) return;
            if (e.ClickCount != 2) return;

            Vertex target = (Vertex)((Ellipse)sender).DataContext;
            DetailDataWindow dialog = new DetailDataWindow();
            dialog.DetailData = target.DetailData;

            if (dialog.ShowDialog() != true) return;

            target.DetailData = dialog.DetailData;

            e.Handled = true;
        }

        private void ellipse_ToolModeConnectNoneSelected(object sender, MouseButtonEventArgs e)
        {
            if (CurrentToolMode != ToolMode.ConnectNoneSelected) return;

            SelectedVertexTail = (Vertex)((Ellipse)sender).DataContext;

            CurrentToolMode = ToolMode.ConnectVertexSelected;

            // Do that to prevent other ellipse events from triggering right after this is finished
            e.Handled = true;
        }

        private void ellipse_ToolModeConnectVertexSelected(object sender, MouseButtonEventArgs e)
        {
            if (CurrentToolMode != ToolMode.ConnectVertexSelected) return;

            Vertex vertexHead = (Vertex)((Ellipse)sender).DataContext;

            Edge result = new Edge();

            EdgePropertiesWindow dialog = new EdgePropertiesWindow();
            dialog.EdgeName = result.Name;
            dialog.IsDirected = result.IsDirected;
            dialog.IsWeighted = result.IsWeighted;
            dialog.WeightDescriptor = result.WeightDescriptor;
            dialog.WeightUnit = result.WeightUnit;
            dialog.WeightValue = result.WeightValue;
            dialog.ViewData = result.ViewData;
            dialog.Tags = result.Tags;

            if (dialog.ShowDialog() != true)
            {
                ClearToolModes();
                return;
            }

            result.Name = dialog.EdgeName;
            result.IsDirected = dialog.IsDirected;

            if (result.IsWeighted = dialog.IsWeighted)
            {
                result.WeightDescriptor = dialog.WeightDescriptor;
                result.WeightUnit = dialog.WeightUnit;
                result.WeightValue = dialog.WeightValue;
            }

            result.ViewData = dialog.ViewData;
            result.Tags = dialog.Tags;

            // Copy field lists of detail data template
            if (dialog.DetailData != null)
            {
                result.DetailData.StringFields = new List<DetailDataField<string>>(dialog.DetailData.StringFields);
                result.DetailData.LongFields = new List<DetailDataField<long>>(dialog.DetailData.LongFields);
                result.DetailData.DecimalFields = new List<DetailDataField<decimal>>(dialog.DetailData.DecimalFields);
                result.DetailData.BoolFields = new List<DetailDataField<bool>>(dialog.DetailData.BoolFields);
                result.DetailData.DateFields = new List<DetailDataField<DateTime>>(dialog.DetailData.DateFields);
            }

            MainGraph.Connect(result, SelectedVertexTail, vertexHead, dialog.IsDirectionReversed);
            UnsavedProgress = true;

            DrawCanvas(true);

            ClearToolModes();

            // Do that to prevent other ellipse events from triggering right after this is finished
            e.Handled = true;
        }

        private void ellipse_ToolModeEdit(object sender, MouseButtonEventArgs e)
        {
            if (CurrentToolMode != ToolMode.Edit) return;

            Vertex target = (Vertex)((Ellipse)sender).DataContext;

            VertexPropertiesWindow dialog = new VertexPropertiesWindow();
            dialog.VertexName = target.Name;
            dialog.Radius = target.Radius;
            dialog.ViewData = target.ViewData;
            dialog.Tags = new List<Tag>(target.Tags);

            // Disable the DetailData template fields, since the template is only used to give the user an initial xml skeleton
            dialog.tDetailDataTemplate.IsEnabled = false;
            dialog.bAddDetailDataTemplate.IsEnabled = false;
            dialog.bEditDetailDataTemplate.IsEnabled = false;
            dialog.bDeleteDetailDataTemplate.IsEnabled = false;
            dialog.cDetailDataTemplate.IsEnabled = false;

            if (dialog.ShowDialog() != true)
            {
                ClearToolModes();
                return;
            }

            target.Name = dialog.VertexName;
            target.Radius = dialog.Radius;
            target.ViewData = dialog.ViewData;
            target.Tags = dialog.Tags;

            UnsavedProgress = true;

            DrawCanvas(true);

            e.Handled = true;

            ClearToolModes();

            // Do that to prevent other ellipse events from triggering right after this is finished
            e.Handled = true;
        }

        private void ellipse_ToolModeDelete(object sender, MouseButtonEventArgs e)
        {
            if (CurrentToolMode != ToolMode.Delete) return;

            Vertex target = (Vertex)((Ellipse)sender).DataContext;

            if (target.Degree > 0 && MessageBox.Show("Deleting this Vertex will result in all Edges incident to it being deleted too. Do you want to proceed?", "Cascading Deletion", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK) != MessageBoxResult.OK)
            {
                ClearToolModes();

                return;
            }

            MainGraph.RemoveVertex(target);
            UnsavedProgress = true;

            DrawCanvas(true);

            e.Handled = true;

            ClearToolModes();

            // Do that to prevent other ellipse events from triggering right after this is finished
            e.Handled = true;
        }

        private void ellipse_ToolModeReconnectEdgeSelected(object sender, MouseButtonEventArgs e)
        {
            if (CurrentToolMode != ToolMode.ReconnectEdgeSelected) return;

            SelectedVertexTail = (Vertex)((Ellipse)sender).DataContext;

            CurrentToolMode = ToolMode.ReconnectEdgeAndVertexSelected;

            // Do that to prevent other ellipse events from triggering right after this is finished
            e.Handled = true;
        }

        private void ellipse_ToolModeReconnectEdgeAndVertexSelected(object sender, MouseButtonEventArgs e)
        {
            if (CurrentToolMode != ToolMode.ReconnectEdgeAndVertexSelected) return;

            Vertex vertexHead = (Vertex)((Ellipse)sender).DataContext;

            MainGraph.Disconnect(SelectedEdge);
            MainGraph.Connect(SelectedEdge, SelectedVertexTail, vertexHead);
            UnsavedProgress = true;

            DrawCanvas(true);

            ClearToolModes();

            // Do that to prevent other ellipse events from triggering right after this is finished
            e.Handled = true;
        }

        private void ellipse_StartDrag(object sender, MouseButtonEventArgs e)
        {
            if (CurrentToolMode != ToolMode.None) return;

            startDrag((Ellipse)sender, e.GetPosition(MainCanvas));
        }

        private void ellipse_EndDrag(object sender, MouseButtonEventArgs e)
        {
            if (ellipseDragTarget == null) return;

            endDrag();
        }

        private void canvas_ellipse_Drag(object sender, MouseEventArgs e)
        {
            if (ellipseDragTarget == null) return;
            // Get new position
            Point mousePosition = Mouse.GetPosition(MainCanvas);

            // Check if the mouse position is outside the canvas and end operation if it is
            if (mousePosition.X < 0 || mousePosition.Y < 0 || mousePosition.X > MainCanvas.Width || mousePosition.Y > MainCanvas.Height)
            {
                endDrag();

                return;
            }

            Vertex target = (Vertex)ellipseDragTarget.DataContext;
            Vector dragOffset = new Vector(mousePosition.X - ellipseLastDragPoint.Value.X, -mousePosition.Y + ellipseLastDragPoint.Value.Y);
            target.Position = target.Position + dragOffset;

            // Check if the new position is outside the canvas and reposition the vertex inside it
            if (target.Position.X - target.Radius < -MainCanvas.Width / 2) target.Position.X = -MainCanvas.Width / 2 + target.Radius;
            else if (target.Position.X + target.Radius > MainCanvas.Width / 2) target.Position.X = MainCanvas.Width / 2 - target.Radius;

            if (target.Position.Y - target.Radius < -MainCanvas.Height / 2) target.Position.Y = -MainCanvas.Height / 2 + target.Radius;
            else if (target.Position.Y + target.Radius > MainCanvas.Height / 2) target.Position.Y = MainCanvas.Height / 2 - target.Radius;

            ellipseLastDragPoint = mousePosition;

            UnsavedProgress = true;

            DrawCanvas(true);

            e.Handled = true;
        }

        private void arrowLine_ShowDetailData(object sender, MouseButtonEventArgs e)
        {
            if (CurrentToolMode != ToolMode.None) return;
            if (e.ClickCount != 2) return;

            Edge target = (Edge)((ArrowLine)sender).DataContext;
            DetailDataWindow dialog = new DetailDataWindow();
            dialog.DetailData = target.DetailData;

            if (dialog.ShowDialog() != true) return;

            target.DetailData = dialog.DetailData;

            e.Handled = true;
        }

        private void arrowLine_ToolModeReconnectNoneSelected(object sender, MouseButtonEventArgs e)
        {
            if (CurrentToolMode != ToolMode.ReconnectNoneSelected) return;

            SelectedEdge = (Edge)((ArrowLine)sender).DataContext;

            CurrentToolMode = ToolMode.ReconnectEdgeSelected;

            // Change arrow cursors so that vertices are highlighted instead of edges
            foreach (ArrowLine arrowLine in MainCanvas.Children.OfType<ArrowLine>()) { arrowLine.Cursor = Cursors.Arrow; }
            foreach (Ellipse ellipse in MainCanvas.Children.OfType<Ellipse>()) { ellipse.Cursor = Cursors.Cross; }

            // Do that to prevent other arrowline events from triggering right after this is finished
            e.Handled = true;
        }

        private void arrowLine_ToolModeEdit(object sender, MouseButtonEventArgs e)
        {
            if (CurrentToolMode != ToolMode.Edit) return;

            Edge target = (Edge)((ArrowLine)sender).DataContext;

            EdgePropertiesWindow dialog = new EdgePropertiesWindow();
            dialog.EdgeName = target.Name;
            dialog.IsDirected = target.IsDirected;
            dialog.IsWeighted = target.IsWeighted;
            dialog.WeightDescriptor = target.WeightDescriptor;
            dialog.WeightUnit = target.WeightUnit;
            dialog.WeightValue = target.WeightValue;
            dialog.ViewData = target.ViewData;
            dialog.Tags = new List<Tag>(target.Tags);

            // Disable the DetailData template fields, since the template is only used to give the user an initial xml skeleton
            dialog.tDetailDataTemplate.IsEnabled = false;
            dialog.bAddDetailDataTemplate.IsEnabled = false;
            dialog.bEditDetailDataTemplate.IsEnabled = false;
            dialog.bDeleteDetailDataTemplate.IsEnabled = false;
            dialog.cDetailDataTemplate.IsEnabled = false;

            if (dialog.ShowDialog() != true)
            {
                ClearToolModes();
                return;
            }

            target.Name = dialog.EdgeName;
            target.IsDirected = dialog.IsDirected;

            if (dialog.IsDirectionReversed) target.ReverseArc();

            if (target.IsWeighted = dialog.IsWeighted)
            {
                target.WeightDescriptor = dialog.WeightDescriptor;
                target.WeightUnit = dialog.WeightUnit;
                target.WeightValue = dialog.WeightValue;
            }

            target.ViewData = dialog.ViewData;
            target.Tags = dialog.Tags;

            UnsavedProgress = true;

            DrawCanvas(true);

            ClearToolModes();

            // Do that to prevent other arrowline events from triggering right after this is finished
            e.Handled = true;
        }

        private void arrowLine_ToolModeDelete(object sender, MouseButtonEventArgs e)
        {
            if (CurrentToolMode != ToolMode.Delete) return;

            Edge target = (Edge)((ArrowLine)sender).DataContext;

            MainGraph.Disconnect(target);
            UnsavedProgress = true;

            DrawCanvas(true);

            ClearToolModes();

            // Do that to prevent other arrowline events from triggering right after this is finished
            e.Handled = true;
        }

        #endregion

        #region RoutedCommand Methods

        private void NewGraph_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearToolModes();

            if (UnsavedProgress)
                if (!HandleUnsavedProgress()) return;

            MainGraph = new Graph();
            UnsavedProgress = false;

            GraphPropertiesWindow dialog = new GraphPropertiesWindow();
            dialog.CanvasSize = MainGraph.CanvasSize;
            dialog.ButtonPanel.Children.Remove(dialog.bCancel);
            dialog.ShowDialog();

            if (dialog.DialogResult == true) MainGraph.CanvasSize = dialog.CanvasSize;

            DrawCanvas(true);
        }

        private void NewGraph_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void OpenGraph_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearToolModes();

            if (UnsavedProgress)
                if (!HandleUnsavedProgress()) return;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".ugraph";
            dialog.Filter = "Ultra Graph|*.ugraph|XML Document|*.xml|Text Document|*.txt|All Files|*.*";
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;

            if (dialog.ShowDialog() != true) return;

            Load(XDocument.Load(dialog.FileName));
        }

        private void OpenGraph_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void SaveGraph_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearToolModes();

            Save();
        }

        private void SaveGraph_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainGraph != null;

            e.Handled = true;
        }

        private void ShowHelp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearToolModes();


        }

        private void ShowHelp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void ExitProgram_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearToolModes();

            if (UnsavedProgress)
                if (!HandleUnsavedProgress()) return;

            Application.Current.Shutdown();

            e.Handled = true;
        }

        private void ExitProgram_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void ShowGraphProperties_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearToolModes();

            GraphPropertiesWindow dialog = new GraphPropertiesWindow();
            dialog.CanvasSize = MainGraph.CanvasSize;
            dialog.ShowDialog();

            if (dialog.DialogResult != true) return;

            MainGraph.CanvasSize = dialog.CanvasSize;
            DrawCanvas(true);
            ScrollToOrigin();

            UnsavedProgress = true;

            e.Handled = true;
        }

        private void ShowGraphProperties_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainGraph != null;

            e.Handled = true;
        }

        private void NewVertexToolMode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearToolModes();

            CurrentToolMode = ToolMode.NewVertex;
            MainCanvas.Cursor = Cursors.Cross;

            e.Handled = true;
        }

        private void NewVertexToolMode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainGraph != null;

            e.Handled = true;
        }

        private void ConnectToolMode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearToolModes();

            CurrentToolMode = ToolMode.ConnectNoneSelected;
            foreach (Ellipse ellipse in MainCanvas.Children.OfType<Ellipse>()) { ellipse.Cursor = Cursors.Cross; }

            e.Handled = true;
        }

        private void ConnectToolMode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainGraph != null;

            e.Handled = true;
        }

        private void ReconnectToolMode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearToolModes();

            CurrentToolMode = ToolMode.ReconnectNoneSelected;
            foreach (ArrowLine arrowLine in MainCanvas.Children.OfType<ArrowLine>()) { arrowLine.Cursor = Cursors.Cross; }

            e.Handled = true;
        }

        private void ReconnectToolMode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainGraph != null;

            e.Handled = true;
        }

        private void EditToolMode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearToolModes();

            CurrentToolMode = ToolMode.Edit;
            foreach (Shape shape in MainCanvas.Children.OfType<Shape>()) { shape.Cursor = Cursors.Cross; }

            e.Handled = true;
        }

        private void EditToolMode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainGraph != null;

            e.Handled = true;
        }

        private void DeleteToolMode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearToolModes();

            CurrentToolMode = ToolMode.Delete;
            foreach (Shape shape in MainCanvas.Children.OfType<Shape>()) { shape.Cursor = Cursors.Cross; }

            e.Handled = true;
        }

        private void DeleteToolMode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainGraph != null;

            e.Handled = true;
        }

        private void UnselectTool_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearToolModes();

            e.Handled = true;
        }

        private void UnselectTool_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainGraph != null;

            e.Handled = true;
        }

        #endregion

        #region DEBUGGING

        /// <summary>
        /// Debugging
        /// </summary>
        /// <returns></returns>
        private Graph DEBUG_CREATE_TEST_GRAPH()
        {
            Graph graph = new Graph();

            Vertex v1 = new Vertex();
            Vertex v2 = new Vertex();
            Vertex v3 = new Vertex();
            Vertex v4 = new Vertex();
            Vertex v5 = new Vertex();

            v1.Name = "Dudelah!";

            v1.Position = new Vector(0, 0);
            v2.Position = new Vector(200, 200);
            v3.Position = new Vector(-200, 200);
            v4.Position = new Vector(-200, -200);
            v5.Position = new Vector(200, -200);

            Edge e1 = new Edge();
            Edge e2 = new Edge();
            Edge e3 = new Edge();
            Edge e4 = new Edge();
            Edge e5 = new Edge();

            e1.IsDirected = true;
            e1.Name = "Edge Name";
            e5.IsDirected = true;

            e2.IsWeighted = true;
            e2.WeightDescriptor = "Mass";
            e2.WeightUnit = "kg";
            e2.WeightValue = 32;

            graph.Connect(e1, v1, v2);
            graph.Connect(e2, v1, v3);
            graph.Connect(e3, v1, v4);
            graph.Connect(e4, v1, v5);
            graph.Connect(e5, v3, v2);

            graph.ToXElement().Save("testOutput.xml");

            Graph graphReloaded = new Graph(XElement.Load("testOutput.xml"));
            graphReloaded.ToXElement().Save("testOutput2.xml");

            return graph;
        }

        #endregion
    }
}
