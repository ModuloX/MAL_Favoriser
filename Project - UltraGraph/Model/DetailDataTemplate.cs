using MarkusRezai.Project.UltraGraph.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MarkusRezai.Project.UltraGraph.Model
{
    public class DetailDataTemplate : GraphData
    {
        public string Name { get; set; }

        public DetailData DetailData { get; set; }

        public DetailDataTemplate()
        {
            Name = "New Detail Data Template";

            DetailData = new DetailData();
        }

        public DetailDataTemplate(XElement xSource)
        {
            Name = xSource.Attribute("Name").Value;

            DetailData = new DetailData(xSource.Element("DetailData"));
        }

        public override XElement ToXElement()
        {
            return new XElement("DetailDataTemplate",
                                new XAttribute("Name", Name),
                                DetailData.ToXElement());
        }
    }
}
