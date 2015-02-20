using MarkusRezai.Project.UltraGraph.Model;
using Petzold.Media2D;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace MarkusRezai.Project.UltraGraph.ViewModel
{
    public static class ViewDataManager
    {
        // Default view data for initializaion of dictionary
        private static VertexViewData defaultVertexViewData = new VertexViewData();
        private static EdgeViewData defaultEdgeViewData = new EdgeViewData();

        // Dictionaries with default view data
        private static Dictionary<int, VertexViewData> vertexViewDataDictionary = new Dictionary<int, VertexViewData>() { { defaultVertexViewDataKey, defaultVertexViewData } };
        private static Dictionary<int, EdgeViewData> edgeViewDataDictionary = new Dictionary<int, EdgeViewData>() { { defaultEdgeViewDataKey, defaultEdgeViewData } };

        public static int defaultVertexViewDataKey { get { return 0; } }
        public static int defaultEdgeViewDataKey { get { return 0; } }

        public static VertexViewData DefaultVertexViewData { get { return defaultVertexViewData; } }
        public static EdgeViewData DefaultEdgeViewData { get { return defaultEdgeViewData; } }

        // Public dictionary properties for adding new view data
        public static Dictionary<int, VertexViewData> VertexViewDataDictionary { get { return vertexViewDataDictionary; } }
        public static Dictionary<int, EdgeViewData> EdgeViewDataDictionary { get { return edgeViewDataDictionary; } }

        // Lists for Binding
        public static List<VertexViewData> VertexViewDataList { get { return vertexViewDataDictionary.Values.ToList(); } }
        public static List<EdgeViewData> EdgeViewDataList { get { return edgeViewDataDictionary.Values.ToList(); } }

        /// <summary>
        /// Loads content for all <see cref="ViewDataManager"/> dictionaries.
        /// </summary>
        /// <param name="viewData">The content provider.</param>
        public static void LoadViewData(XElement viewData)
        {
            // Clear dictionary and get VertexEllipses
            vertexViewDataDictionary.Clear();
            foreach (XElement vertexEllipse in viewData.Element("VertexEllipses").Elements("VertexEllipse"))
            {
                vertexViewDataDictionary.Add(int.Parse(vertexEllipse.Element("Id").Value),
                                             new VertexViewData(vertexEllipse.Element("ViewData")));
            }

            // Clear dictionary and get EdgeArrowLines
            edgeViewDataDictionary.Clear();
            foreach (XElement edgeArrowLine in viewData.Element("EdgeArrowLines").Elements("EdgeArrowLine"))
            {
                edgeViewDataDictionary.Add(int.Parse(edgeArrowLine.Element("Id").Value),
                                           new EdgeViewData(edgeArrowLine.Element("ViewData")));
            }
        }

        /// <summary>
        /// Converts the <see cref="ViewDataManager"/> into XML code.
        /// </summary>
        /// <returns>The XML representation of the <see cref="ViewDataManager"/>.</returns>
        public static XElement ToXElement()
        {
            // Default Elements
            XElement result = new XElement("ViewDataDictionary",
                                           new XElement("VertexEllipses"),
                                           new XElement("EdgeArrowLines"));

            // Add VertexEllipses
            foreach (KeyValuePair<int, VertexViewData> dictionaryElement in vertexViewDataDictionary)
            {
                result.Element("VertexEllipses").Add(new XElement("VertexEllipse",
                                                                  new XElement("Id", dictionaryElement.Key),
                                                                  dictionaryElement.Value.ToXElement()
                                                                  ));
            }

            // Add EdgeArrowLines
            foreach (KeyValuePair<int, EdgeViewData> dictionaryElement in edgeViewDataDictionary)
            {
                result.Element("EdgeArrowLines").Add(new XElement("EdgeArrowLine",
                                                                  new XElement("Id", dictionaryElement.Key),
                                                                  dictionaryElement.Value.ToXElement()
                                                                  ));
            }

            return result;
        }

        /// <summary>
        /// Applies the <see cref="VertexViewData"/> of a <see cref="Vertex"/> to an <see cref="Ellipse"/>.
        /// </summary>
        /// <param name="vertex">The <see cref="Vertex"/> with the desired <see cref="VertexViewData"/>.</param>
        /// <param name="ellipse">The <see cref="Ellipse"/> that will be changed.</param>
        public static void ApplyVertexEllipseViewData(Vertex vertex, Ellipse ellipse)
        {
            // Assign default view data to vertex if it has none
            if (vertex.ViewData == null) vertex.ViewData = ViewDataManager.defaultVertexViewData;

            ellipse.Fill = vertex.ViewData.FillBrush;
            ellipse.Stroke = vertex.ViewData.StrokeBrush;
            ellipse.StrokeThickness = vertex.ViewData.StrokeThickness;
            ellipse.StrokeDashArray = vertex.ViewData.StrokeDashes;


        }

        /// <summary>
        /// Applies the <see cref="EdgeViewData"/> of a <see cref="Edge"/> to an <see cref="ArrowLine"/>.
        /// </summary>
        /// <param name="edge">The <see cref="Edge"/> with the desired <see cref="EdgeViewData"/>.</param>
        /// <param name="arrowLine">The <see cref="ArrowLine"/> that will be changed.</param>
        public static void ApplyEdgeArrowLineViewData(Edge edge, ArrowLine arrowLine)
        {
            // Assign default view data to edge if it has none
            if (edge.ViewData == null) edge.ViewData = ViewDataManager.defaultEdgeViewData;

            arrowLine.Stroke = edge.ViewData.StrokeBrush;
            arrowLine.StrokeThickness = edge.ViewData.StrokeThickness;

            // The fill brush may only be set if the arrowhead is closed, here both properties are set
            if (arrowLine.IsArrowClosed = edge.ViewData.IsArrowHeadClosed) arrowLine.Fill = edge.ViewData.StrokeBrush;

            arrowLine.ArrowAngle = edge.ViewData.ArrowAngle;
            arrowLine.ArrowLength = edge.ViewData.ArrowLength;
        }
    }
}
