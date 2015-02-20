using MarkusRezai.Project.UltraGraph.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MarkusRezai.Project.UltraGraph.Model
{
    public class DetailDataField<T> : DetailDataBaseField, ITaggable
    {
        public T Value { get; set; }

        public DetailDataField(string name, List<Tag> tags, int displayIndex, T value)
        {
            Name = name;
            DisplayIndex = displayIndex;
            Value = value;
            Tags = tags;
        }

        public DetailDataField(XElement xSource, T value)
        {
            Name = xSource.Attribute("Name").Value;
            DisplayIndex = int.Parse(xSource.Attribute("DisplayIndex").Value);
            Value = value;

            Tags = (from xTag in xSource.Element("Tags").Elements("Tag") select new Tag(xTag)).ToList();
        }

        public override XElement ToXElement()
        {
            XElement xTags = new XElement("Tags");
            foreach (Tag tag in Tags) xTags.Add(tag.ToXElement());

            return new XElement("Field",
                                new XAttribute("Name", Name),
                                new XAttribute("DisplayIndex", DisplayIndex),
                                new XAttribute("Value", Value.ToString()),
                                xTags
                                );
        }
    }
}
