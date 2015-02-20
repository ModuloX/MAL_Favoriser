using MarkusRezai.Project.UltraGraph.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MarkusRezai.Project.UltraGraph.Model
{
    public class Edge : GraphData, ITaggable
    {
        #region ViewProperties

        /// <summary>
        /// Only used for dragging operation, to avoid glitches
        /// </summary>
        public int ZIndex { get; set; }

        /// <summary>
        /// Defines view properties unique to this edge.
        /// </summary>
        public EdgeViewData ViewData { get; set; }
        /// <summary>
        /// Defines the detail view of this Vertex.
        /// </summary>
        public DetailData DetailData { get; set; }

        #endregion

        public long ComponentId { get; set; }
        public List<Tag> Tags { get; set; }

        /// <summary>
        /// The descriptive name of this <see cref="Edge"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The first Vertex of an Edge, if the Edge is weighted this is the tail.
        /// </summary>
        public Vertex VertexA { get; set; }
        /// <summary>
        /// The second Vertex of an Edge, if the Edge is weighted this is the head.
        /// </summary>
        public Vertex VertexB { get; set; }

        /// <summary>
        /// A weighted graph associates a label (weight) with every edge in the graph. Here, it is possible to have
        /// weighted and unweighted Edges in the same Graph, algorithms will only be affected by weight if the whole Graph
        /// is weighted.
        /// </summary>
        public bool IsWeighted { get; set; }
        /// <summary>
        /// The descriptor of the Edge weight, eg. cost or length.
        /// </summary>
        public string WeightDescriptor { get; set; }
        /// <summary>
        /// The unit of the Edge weight, eg. kg or km.
        /// </summary>
        public string WeightUnit { get; set; }
        /// <summary>
        /// The value of the Edge weight.
        /// </summary>
        public long WeightValue { get; set; }

        /// <summary>
        /// The multiplicity of an edge is the number of edges sharing the same end vertices.
        /// </summary>
        public int Multiplicity { get { return VertexA.Edges.Count(x => (x.VertexA == VertexA && x.VertexB == VertexB) || (x.VertexA == VertexB && x.VertexB == VertexA)); } }
        /// <summary>
        /// An edge is multiple if there is another edge with the same endvertices; otherwise it is simple. 
        /// </summary>
        public bool IsSimple { get { return Multiplicity == 1; } }

        /// <summary>
        /// An Edge is only valid if its VertexA and VertexB are set. Validity should always be checked before calling
        /// other properties or methods, to prevent null pointer exceptions.
        /// </summary>
        public bool IsValid { get { return VertexA != null && VertexB != null; } }
        /// <summary>
        /// A loop is an edge whose endpoints are the same vertex.
        /// </summary>
        public bool IsLoop { get { return VertexA == VertexB; } }

        /// <summary>
        /// A arc, or directed edge, is an ordered pair of endvertices that can be represented graphically as 
        /// an arrow drawn between the endvertices. In such an ordered pair the first vertex is called the initial vertex 
        /// or tail; the second one is called the terminal vertex or head (because it appears at the arrow head).
        /// Here, VertexA is always the tail, VertexB is always the head.
        /// </summary>
        public bool IsDirected { get; set; }

        public Edge()
        {
            ZIndex = 10;

            ViewData = ViewDataManager.DefaultEdgeViewData;
            DetailData = new DetailData();

            ComponentId = -1;
            Tags = new List<Tag>();

            Name = "";

            IsWeighted = false;
            WeightDescriptor = "";
            WeightUnit = "";
            WeightValue = -1;

            IsDirected = false;
        }

        public Edge(XElement xSource)
        {
            ZIndex = 10;

            // Try to get the specified viewdata from the viewdatamanger; assign the defaultviewdata if it cant be found
            EdgeViewData viewData = ViewDataManager.DefaultEdgeViewData;
            ViewDataManager.EdgeViewDataDictionary.TryGetValue(int.Parse(xSource.Attribute("ViewData").Value), out viewData);
            ViewData = viewData;

            DetailData = new DetailData(xSource.Element("DetailData"));

            ComponentId = long.Parse(xSource.Attribute("ComponentId").Value);
            Tags = (from xTag in xSource.Element("Tags").Elements("Tag") select new Tag(xTag)).ToList();

            Name = xSource.Attribute("Name").Value;

            IsWeighted = bool.Parse(xSource.Attribute("IsWeighted").Value);
            WeightDescriptor = xSource.Attribute("WeightDescriptor").Value;
            WeightUnit = xSource.Attribute("WeightUnit").Value;
            WeightValue = long.Parse(xSource.Attribute("WeightValue").Value);

            IsDirected = bool.Parse(xSource.Attribute("IsDirected").Value);
        }

        /// <summary>
        /// Returns the other Vertex of this Edge, or, if the parameter Vertex is not incident to this Edge, null.
        /// </summary>
        public Vertex GetCounterpart(Vertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException();

            if (vertex == VertexA) return VertexB;
            else if (vertex == VertexB) return VertexA;
            else return null;
        }

        /// <summary>
        /// If this Edge is directed, it has a head and a tail that is, this method is used to invert its direction. <para/>
        /// Since VertexA is always the tail and VertexB is always the head, this method simplay swaps the references.
        /// </summary>
        public void ReverseArc()
        {
            if (!IsDirected) return;

            Vertex temp = VertexA;
            VertexA = VertexB;
            VertexB = temp;
        }

        /// <summary>
        /// Returns the XML representations of this Edge.
        /// </summary>
        /// <returns>An XElement representing this Edge. Returns null if this Edge is invalid.</returns>
        public override XElement ToXElement()
        {
            if (!IsValid) return null;

            XElement xTags = new XElement("Tags");
            foreach (Tag tag in Tags) xTags.Add(tag.ToXElement());

            // Look up the key for the viewdata and set it to the default one in case the viewdata was deleted from the viewdatamanager
            int viewDataKey = ViewDataManager.defaultEdgeViewDataKey;
            if (ViewDataManager.EdgeViewDataDictionary.ContainsValue(ViewData)) viewDataKey = ViewDataManager.EdgeViewDataDictionary.Single(x => x.Value == ViewData).Key;

            return new XElement("Edge",
                                new XAttribute("ComponentId", ComponentId),
                                new XAttribute("Name", Name),
                                new XAttribute("VertexA", VertexA.ComponentId),
                                new XAttribute("VertexB", VertexB.ComponentId),
                                new XAttribute("IsDirected", IsDirected),
                                new XAttribute("IsWeighted", IsWeighted),
                                new XAttribute("WeightDescriptor", WeightDescriptor),
                                new XAttribute("WeightUnit", WeightUnit),
                                new XAttribute("WeightValue", WeightValue),
                                new XAttribute("ViewData", viewDataKey),
                                DetailData.ToXElement(),
                                xTags
                                );
        }
    }
}
