using MarkusRezai.Project.UltraGraph.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace MarkusRezai.Project.UltraGraph.ViewModel
{
    public class VertexViewData : GraphData
    {
        public string Name { get; set; }

        public SolidColorBrush FillBrush { get; set; }
        public SolidColorBrush StrokeBrush { get; set; }
        public double StrokeThickness { get; set; }
        public DoubleCollection StrokeDashes { get; set; }

        public VertexViewData()
        {
            Name = "Default Style";

            FillBrush = new SolidColorBrush(Color.FromArgb(255, 150, 150, 255));
            StrokeBrush = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
            StrokeThickness = 3;
            StrokeDashes = new DoubleCollection(new List<double>() { });
        }

        public VertexViewData(XElement xSource)
        {
            Name = xSource.Attribute("Name").Value;

            FillBrush = new SolidColorBrush(Color.FromArgb(byte.Parse(xSource.Element("FillBrush").Attribute("A").Value),
                                                           byte.Parse(xSource.Element("FillBrush").Attribute("R").Value),
                                                           byte.Parse(xSource.Element("FillBrush").Attribute("G").Value),
                                                           byte.Parse(xSource.Element("FillBrush").Attribute("B").Value)));

            StrokeBrush = new SolidColorBrush(Color.FromArgb(byte.Parse(xSource.Element("StrokeBrush").Attribute("A").Value),
                                                             byte.Parse(xSource.Element("StrokeBrush").Attribute("R").Value),
                                                             byte.Parse(xSource.Element("StrokeBrush").Attribute("G").Value),
                                                             byte.Parse(xSource.Element("StrokeBrush").Attribute("B").Value)));

            StrokeThickness = double.Parse(xSource.Element("StrokeThickness").Value);

            StrokeDashes = new DoubleCollection(new List<double>() { });
            foreach (XElement doubleValue in xSource.Element("StrokeDashes").Elements("DoubleValue")) StrokeDashes.Add(double.Parse(doubleValue.Value));
        }

        public override XElement ToXElement()
        {
            XElement xDashes = new XElement("StrokeDashes",
                                           new XComment("Each Double in the collection specifies the length of a dash or gap relative to the Thickness of the pen."),
                                           new XComment("For example, a value of 1 creates a dash or gap that has the same length as the thickness of the pen (a square)."),
                                           new XComment("Starting at index 0, elements with an even index value specify dashes; objects with an odd index value specify gaps.")
                                           );

            foreach (double value in StrokeDashes) xDashes.Add(new XElement("DoubleValue", value));

            return new XElement("ViewData",
                                new XAttribute("Name", Name),
                                new XElement("FillBrush",
                                    new XAttribute("A", FillBrush.Color.A),
                                    new XAttribute("R", FillBrush.Color.R),
                                    new XAttribute("G", FillBrush.Color.G),
                                    new XAttribute("B", FillBrush.Color.B)),
                                new XElement("StrokeBrush",
                                    new XAttribute("A", StrokeBrush.Color.A),
                                    new XAttribute("R", StrokeBrush.Color.R),
                                    new XAttribute("G", StrokeBrush.Color.G),
                                    new XAttribute("B", StrokeBrush.Color.B)),
                                new XElement("StrokeThickness", StrokeThickness),
                                xDashes
                                );
        }

    }
}
