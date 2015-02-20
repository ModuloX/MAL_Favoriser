using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MarkusRezai.Project.UltraGraph.Model
{
    public class Tag
    {
        /// <summary>
        /// Text which is displayed for a specific Tag. This string is not used when searching for a Tag.
        /// </summary>
        public string DisplayText { get; set; }
        /// <summary>
        /// Values which are used to identify a Tag. Those strings are used when searching for a Tag.
        /// </summary>
        public List<string> TagValues { get; set; }
        /// <summary>
        /// Used for WPF binding
        /// </summary>
        public string Text
        {
            get
            {
                StringBuilder result = new StringBuilder(DisplayText + ": ");

                foreach (string tagValue in TagValues) result.Append(tagValue).Append(" ; ");

                return result.ToString();
            }
        }

        public Tag()
        {
            DisplayText = "New Tag";
            TagValues = new List<string>();
        }

        public Tag(XElement xSource)
        {
            DisplayText = xSource.Attribute("DisplayText").Value;
            TagValues = (from tv in xSource.Element("TagValues").Elements("TagValue") select tv.Value).ToList();
        }

        public XElement ToXElement()
        {
            XElement xTagValues = new XElement("TagValues");
            foreach (string tagValue in TagValues) xTagValues.Add(new XElement("TagValue", tagValue));

            return new XElement("Tag",
                                new XAttribute("DisplayText", DisplayText),
                                xTagValues);
        }
    }
}
