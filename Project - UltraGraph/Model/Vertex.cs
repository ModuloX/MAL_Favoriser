using MarkusRezai.Project.UltraGraph.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MarkusRezai.Project.UltraGraph.Model
{
    public class Vertex : GraphData, ITaggable
    {
        #region ViewProperties

        /// <summary>
        /// Only used for dragging operation, to avoid glitches
        /// </summary>
        public int ZIndex { get; set; }

        /// <summary>
        /// Only used to determine the position on the <see cref="MainWindow.DrawingCanvas"/>.
        /// </summary>
        public Vector Position;
        public double Radius { get; set; }
        /// <summary>
        /// Defines view properties unique to this edge.
        /// </summary>
        public VertexViewData ViewData { get; set; }
        /// <summary>
        /// Defines the detail view of this Vertex.
        /// </summary>
        public DetailData DetailData { get; set; }

        #endregion

        public long ComponentId { get; set; }
        public List<Tag> Tags { get; set; }

        /// <summary>
        /// The descriptive name of this <see cref="Vertex"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// All Edges incident to this Vertex.
        /// </summary>
        public List<Edge> Edges { get; set; }

        /// <summary>
        /// Two vertices u and v are called adjacent if an edge exists between them. The set of neighbors of v, that is, 
        /// vertices adjacent to v not including v itself, forms  the open neighborhood of v. When v is also included, it
        /// is called a closed neighborhood.
        /// </summary>
        public List<Vertex> OpenNeighbours { get { return Edges.Where(x => !x.IsLoop).Select(x => x.GetCounterpart(this)).Distinct().ToList<Vertex>(); } }
        /// <summary>
        /// Two vertices u and v are called adjacent if an edge exists between them. The set of neighbors of v, that is, 
        /// vertices adjacent to v not including v itself, forms  the open neighborhood of v. When v is also included, it
        /// is called a closed neighborhood.
        /// </summary>
        public List<Vertex> ClosedNeighbours { get { return Edges.Select(x => x.GetCounterpart(this)).Distinct().ToList<Vertex>(); } }

        /// <summary>
        /// The degree of a vertex in a graph is the number of edges incident to it, with loops being counted twice.
        /// </summary>
        public int Degree { get { return Edges.Count + Edges.Where(x => x.IsLoop).Count(); } }

        public Vertex()
        {
            ZIndex = 20;

            Position = new Vector(0, 0);
            Radius = 40;
            ViewData = ViewDataManager.DefaultVertexViewData;
            DetailData = new DetailData();

            ComponentId = -1;
            Tags = new List<Tag>();

            Name = "";

            Edges = new List<Edge>();
        }

        public Vertex(XElement xSource)
        {
            ZIndex = 20;

            Position = new Vector(double.Parse(xSource.Attribute("PositionX").Value), double.Parse(xSource.Attribute("PositionY").Value));
            Radius = double.Parse(xSource.Attribute("Radius").Value);

            // Try to get the specified viewdata from the viewdatamanger; assign the defaultviewdata if it cant be found
            VertexViewData viewData = ViewDataManager.DefaultVertexViewData;
            ViewDataManager.VertexViewDataDictionary.TryGetValue(int.Parse(xSource.Attribute("ViewData").Value), out viewData);
            ViewData = viewData;

            DetailData = new DetailData(xSource.Element("DetailData"));

            ComponentId = long.Parse(xSource.Attribute("ComponentId").Value);
            Tags = (from xTag in xSource.Element("Tags").Elements("Tag") select new Tag(xTag)).ToList();

            Name = xSource.Attribute("Name").Value;

            Edges = new List<Edge>();
        }

        /// <summary>
        /// Converts this vertex into an XElement.
        /// </summary>
        /// <returns>The XML representation of this vertex.</returns>
        public override XElement ToXElement()
        {
            XElement xTags = new XElement("Tags");
            foreach (Tag tag in Tags) xTags.Add(tag.ToXElement());

            // Look up the key for the viewdata and set it to the default one in case the viewdata was deleted from the viewdatamanager
            int viewDataKey = ViewDataManager.defaultVertexViewDataKey;
            if (ViewDataManager.VertexViewDataDictionary.ContainsValue(ViewData)) viewDataKey = ViewDataManager.VertexViewDataDictionary.Single(x => x.Value == ViewData).Key;

            return new XElement("Vertex",
                                new XAttribute("ComponentId", ComponentId),
                                new XAttribute("Name", Name),
                                new XAttribute("PositionX", Position.X),
                                new XAttribute("PositionY", Position.Y),
                                new XAttribute("Radius", Radius),
                                new XAttribute("ViewData", viewDataKey),
                                DetailData.ToXElement(),
                                xTags
                                );
        }
    }
}
