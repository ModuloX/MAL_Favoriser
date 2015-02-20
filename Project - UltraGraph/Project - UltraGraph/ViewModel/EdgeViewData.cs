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
    public class EdgeViewData : GraphData
    {
        public string Name { get; set; }

        public SolidColorBrush StrokeBrush { get; set; }
        public double StrokeThickness { get; set; }
        public bool IsArrowHeadClosed { get; set; }
        public double ArrowAngle { get; set; }
        public double ArrowLength { get; set; }

        public EdgeViewData()
        {
            Name = "Default Style";

            StrokeBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            StrokeThickness = 3;
            IsArrowHeadClosed = true;
            ArrowAngle = 35;
            ArrowLength = 15;
        }

        public EdgeViewData(XElement xSource)
        {
            Name = xSource.Attribute("Name").Value;

            StrokeBrush = new SolidColorBrush(Color.FromArgb(byte.Parse(xSource.Element("StrokeBrush").Attribute("A").Value),
                                                             byte.Parse(xSource.Element("StrokeBrush").Attribute("R").Value),
                                                             byte.Parse(xSource.Element("StrokeBrush").Attribute("G").Value),
                                                             byte.Parse(xSource.Element("StrokeBrush").Attribute("B").Value)));

            StrokeThickness = double.Parse(xSource.Element("StrokeThickness").Value);
            IsArrowHeadClosed = bool.Parse(xSource.Element("IsArrowHeadClosed").Value);
            ArrowAngle = double.Parse(xSource.Element("ArrowAngle").Value);
            ArrowLength = double.Parse(xSource.Element("ArrowLength").Value);
        }

        public override XElement ToXElement()
        {
            return new XElement("ViewData",
                                new XAttribute("Name", Name),
                                new XElement("StrokeBrush",
                                    new XAttribute("A", StrokeBrush.Color.A),
                                    new XAttribute("R", StrokeBrush.Color.R),
                                    new XAttribute("G", StrokeBrush.Color.G),
                                    new XAttribute("B", StrokeBrush.Color.B)),
                                new XElement("StrokeThickness", StrokeThickness),
                                new XElement("IsArrowHeadClosed", IsArrowHeadClosed),
                                new XElement("ArrowAngle", ArrowAngle),
                                new XElement("ArrowLength", ArrowLength)
                                );
        }
    }
}
