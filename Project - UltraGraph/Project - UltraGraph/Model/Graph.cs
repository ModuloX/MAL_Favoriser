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
    public class Graph : GraphData
    {
        #region ViewProperties



        /// <summary>
        /// The size of the canvas this graph will be displayed on.
        /// </summary>
        public Size CanvasSize { get; set; }



        #endregion



        /// <summary>
        /// Represents all Edges of this Graph. Do not add or remove edges nor edit connections directly, use <see cref="Graph.Connect"/> and <see cref="Graph.Disconnect"/>.
        /// </summary>
        public List<Edge> Edges { get; private set; }
        /// <summary>
        /// Represents all Vertices of this Graph. Do not remove vertices or edit connections directly, use <see cref="Graph.Connect"/> and <see cref="Graph.Disconnect"/>. Simply adding them should be okay, though (but you should use <see cref="Graph.AddVertex"/>).
        /// </summary>
        public List<Vertex> Vertices { get; private set; }

        /// <summary>
        /// Returns the highes ComponentId of all Edges and Vertices of this Graph.
        /// </summary>
        public long LastComponentId { get { return Math.Max(Vertices.Count == 0 ? 0 : Vertices.Max(x => x.ComponentId), Edges.Count == 0 ? 0 : Edges.Max(x => x.ComponentId)); } }

        /// <summary>
        /// The size of a graph is the number of its edges.
        /// </summary>
        public int Size { get { return Edges.Count; } }
        /// <summary>
        /// The order of a graph is the number of its vertices.
        /// </summary>
        public int Order { get { return Vertices.Count; } }

        /// <summary>
        /// The total degree of a graph is the sum of the degrees of all its vertices.
        /// </summary>
        public int TotalDegree { get { return Vertices.Sum(x => x.Degree); } }

        /// <summary>
        /// The multiplicity of a graph is the maximum multiplicity of its edges.
        /// </summary>
        public int Multiplicity { get { return Edges.Max(x => x.Multiplicity); } }

        /// <summary>
        /// A graph is a simple graph if it has no multiple edges or loops.
        /// </summary>
        public bool IsSimple { get { return Edges.TrueForAll(x => x.IsSimple && !x.IsLoop); } }
        /// <summary>
        /// A weighted graph associates a label (weight) with every edge in the graph.
        /// </summary>
        public bool IsWeighted { get { return Edges.TrueForAll(x => x.IsWeighted); } }
        /// <summary>
        /// A directed graph (or digraph) is a graph, where all edges have a direction associated with them.
        /// </summary>
        public bool IsDirected { get { return Edges.TrueForAll(x => x.IsDirected); } }

        /// <summary>
        /// If it is possible to establish a path from any vertex to any other vertex of a graph, the graph is 
        /// connected; otherwise, the graph is disconnected.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                // If there are less than 2 Vertices, the Graph is always connected
                // If there are 2 or more Vertices, a DFS discovers connected Vertices, starting from the first in the List and if it discovers all vertices, the graph is connected
                if (Order < 2) return true;

                List<Vertex> discoveredVertices = new List<Vertex>();
                Stack<Vertex> nextVertices = new Stack<Vertex>();

                // The first Vertex of this Graph is pushed onto the Stack
                nextVertices.Push(Vertices.First());
                while (nextVertices.Count > 0)
                {
                    // The uppermost Vertex of the stack is popped out and if it is not already discovered its neighbours are pushed onto the stack
                    Vertex currentVertex = nextVertices.Pop();
                    if (!discoveredVertices.Contains(currentVertex))
                    {
                        discoveredVertices.Add(currentVertex);
                        currentVertex.OpenNeighbours.ForEach(x => nextVertices.Push(x));
                    }
                }

                //The number of discovered Vertices must match the Order of this Graph
                return discoveredVertices.Count == Order;
            }
        }
        /// <summary>
        /// A cyclic graph is a graph containing at least one graph cycle. A graph that is not cyclic is said to be acyclic.
        /// </summary>
        public bool IsCyclic
        {
            get
            {
                throw new NotImplementedException("Über DFS algorithmus back edges finden, wichtig: zwischen directed und non directed edges unterscheiden!");
            }
        }

        public Graph()
        {
            Edges = new List<Edge>();
            Vertices = new List<Vertex>();
            CanvasSize = new Size(10000, 10000);
        }

        public Graph(XElement xSource)
        {
            // Construct Vertices and add them to this.Vertices
            // It is absolutely vital to add them manually and not by using Graph.AddVertex, since the ComponentId must not change to preserve consistency between saving and loading.
            Vertices = (from xVertex in xSource.Element("Vertices").Elements("Vertex") select new Vertex(xVertex)).ToList();

            // Construct Edges and connect them
            // It is absolutely vital to add them manually and not by using Graph.Connect, since the ComponentId must not change to preserve consistency between saving and loading.
            Edges = new List<Edge>();
            foreach (XElement xEdge in xSource.Element("Edges").Elements("Edge"))
            {
                Edge edge = new Edge(xEdge)
                {
                    VertexA = Vertices.Single(x => x.ComponentId == long.Parse(xEdge.Attribute("VertexA").Value)),
                    VertexB = Vertices.Single(x => x.ComponentId == long.Parse(xEdge.Attribute("VertexB").Value))
                };

                Edges.Add(edge);

                // If the Edge is a loop, only add it once to the Vertex, to avoid duplicate entries in vertex.Edges
                edge.VertexA.Edges.Add(edge);
                if (!edge.IsLoop) edge.VertexB.Edges.Add(edge);
            }

            CanvasSize = new Size(double.Parse(xSource.Attribute("CanvasSizeWidth").Value),
                                  double.Parse(xSource.Attribute("CanvasSizeHeight").Value));
        }

        /// <summary>
        /// Adds a Vertex without any Edges to this Graph.<para/>
        /// Attention: The ComponentId is set to a new value.
        /// </summary>
        /// <param name="vertex">The Vertex to be added.</param>
        public void AddVertex(Vertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException();
            if (Vertices.Contains(vertex)) return;

            vertex.ComponentId = LastComponentId + 1;
            Vertices.Add(vertex);
        }

        /// <summary>
        /// Removes a Vertex from this Graph.<para/>
        /// Attention: All Edges connected to that Vertex are disconnected and removed as well.
        /// </summary>
        /// <param name="vertex">The Vertex to be removed.</param>
        public void RemoveVertex(Vertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException();
            if (!Vertices.Contains(vertex)) return;

            // Disconnect all edges
            if (vertex.Edges.Count > 0)
            {
                foreach (Edge edge in vertex.Edges)
                {
                    if (!edge.IsLoop) edge.GetCounterpart(vertex).Edges.Remove(edge);

                    edge.VertexA = null;
                    edge.VertexB = null;

                    Edges.Remove(edge);
                }

                vertex.Edges.Clear();
            }

            // Remove vertex
            Vertices.Remove(vertex);
        }

        /// <summary>
        /// Connects two vertices via an edge. Use this method to add edges (and vertices) or edit connections, to preserve the consistency of the graph.<para/>
        /// Attention: The ComponentId of both Vertices and the Edge are set to new values.
        /// </summary>
        /// <param name="edge">The edge used to connect the vertices. If this edge is already connected, 
        /// that is contained in the edges of this graph or a valid edge in another graph, an exception is thrown.</param>
        /// <param name="vertexA">The first vertex that will be connected. If the edge is directed, this vertex will be the tail.</param>
        /// <param name="vertexB">The second vertex that will be connected. If the edge is directed, this vertex will be the head.</param>
        public void Connect(Edge edge, Vertex vertexA, Vertex vertexB)
        {
            if (edge == null) throw new ArgumentNullException();
            if (vertexA == null) throw new ArgumentNullException();
            if (vertexB == null) throw new ArgumentNullException();

            if (Edges.Contains(edge) || edge.IsValid) throw new ArgumentException("This edge is already connected!");

            long lastComponentId = LastComponentId;

            if (!Vertices.Contains(vertexA))
            {
                vertexA.ComponentId = ++lastComponentId;
                Vertices.Add(vertexA);
            }

            if (!Vertices.Contains(vertexB))
            {
                vertexB.ComponentId = ++lastComponentId;
                Vertices.Add(vertexB);
            }

            edge.ComponentId = ++lastComponentId;
            Edges.Add(edge);

            edge.VertexA = vertexA;
            edge.VertexB = vertexB;

            // If the Edge is a loop, only add it once to the Vertex, to avoid duplicate entries in vertex.Edges
            vertexA.Edges.Add(edge);
            if (!edge.IsLoop) vertexB.Edges.Add(edge);
        }
        
        /// <summary>
        /// Connects two vertices via an edge. Use this method to add edges (and vertices) or edit connections, to preserve the consistency of the graph.<para/>
        /// Attention: The ComponentId of both Vertices and the Edge are set to new values.
        /// </summary>
        /// <param name="edge">The edge used to connect the vertices. If this edge is already connected, 
        /// that is contained in the edges of this graph or a valid edge in another graph, an exception is thrown.</param>
        /// <param name="vertexA">The first vertex that will be connected. If the edge is directed, this vertex will be the tail.</param>
        /// <param name="vertexB">The second vertex that will be connected. If the edge is directed, this vertex will be the head.</param>
        /// <param name="reverseDirection">Indicates whether the direction will be reversed. If it is reversed <paramref name="vertexA"/> 
        ///                                will be the head and <paramref name="vertexB"/> will be the tail. Note that in this case the parameters are just assigned differently
        ///                                to the class members, meaning that <see cref="Edge.VertexA"/> will still be the tail and <see cref="Edge.VertexB"/> the head.</param>
        public void Connect(Edge edge, Vertex vertexA, Vertex vertexB, bool reverseDirection)
        {
            if (edge == null) throw new ArgumentNullException();
            if (vertexA == null) throw new ArgumentNullException();
            if (vertexB == null) throw new ArgumentNullException();

            if (Edges.Contains(edge) || edge.IsValid) throw new ArgumentException("This edge is already connected!");

            long lastComponentId = LastComponentId;

            if (!Vertices.Contains(vertexA))
            {
                vertexA.ComponentId = ++lastComponentId;
                Vertices.Add(vertexA);
            }

            if (!Vertices.Contains(vertexB))
            {
                vertexB.ComponentId = ++lastComponentId;
                Vertices.Add(vertexB);
            }

            edge.ComponentId = ++lastComponentId;
            Edges.Add(edge);

            if (reverseDirection)
            {
                edge.VertexA = vertexB;
                edge.VertexB = vertexA;
            }
            else
            {
                edge.VertexA = vertexA;
                edge.VertexB = vertexB;
            }

            // If the Edge is a loop, only add it once to the Vertex, to avoid duplicate entries in vertex.Edges
            vertexA.Edges.Add(edge);
            if (!edge.IsLoop) vertexB.Edges.Add(edge);
        }

        /// <summary>
        /// Disconnects two vertices via an edge. Use this method to remove edges or edit connections, to preserve the consistency of the graph.
        /// </summary>
        /// <param name="edge">The edge that is going to be removed.</param>
        public void Disconnect(Edge edge)
        {
            if (edge == null) throw new ArgumentNullException();

            if (!Edges.Contains(edge)) throw new ArgumentException("That edge is not connected or part of another graph!");

            edge.VertexA.Edges.Remove(edge);
            if (!edge.IsLoop) edge.VertexB.Edges.Remove(edge);

            edge.VertexA = null;
            edge.VertexB = null;

            Edges.Remove(edge);
        }

        /// <summary>
        /// Converts this graph into an XElement.
        /// </summary>
        /// <returns>The XML representation of this graph.</returns>
        public override XElement ToXElement()
        {
            XElement xVertices = new XElement("Vertices");
            foreach (Vertex vertex in Vertices) xVertices.Add(vertex.ToXElement());

            XElement xEdges = new XElement("Edges");
            foreach (Edge edge in Edges) xEdges.Add(edge.ToXElement());

            return new XElement("GraphData",
                                new XAttribute("CanvasSizeWidth", CanvasSize.Width),
                                new XAttribute("CanvasSizeHeight", CanvasSize.Height),
                                xEdges,
                                xVertices);
        }
    }
}
