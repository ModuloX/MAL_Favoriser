using MarkusRezai.Project.UltraGraph.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MarkusRezai.Project.UltraGraph.Model
{
    /// <summary>
    /// Only used to make a list of various fields for display in a listbox
    /// </summary>
    public class DetailDataBaseField : GraphData
    {
        public string Name { get; set; }
        public int DisplayIndex { get; set; }
        public List<Tag> Tags { get; set; }

        public override XElement ToXElement()
        {
            XElement xTags = new XElement("Tags");
            foreach (Tag tag in Tags) xTags.Add(tag.ToXElement());

            return new XElement("DetailDataBaseField",
                                new XAttribute("Name", Name),
                                new XAttribute("DisplayIndex", DisplayIndex),
                                xTags
                                );
        }
    }
}
