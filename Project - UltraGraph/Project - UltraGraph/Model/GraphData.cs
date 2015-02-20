using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MarkusRezai.Project.UltraGraph.Model
{
    public abstract class GraphData
    {
        public GraphData()
        {

        }

        public GraphData(XElement xSource)
        {

        }

        /// <summary>
        /// Converts this <see cref="GraphData"/> object to an <see cref="XElement"/>.
        /// </summary>
        /// <returns>The <see cref="XElement"/> representation of this <see cref="GraphData"/> object.</returns>
        public abstract XElement ToXElement();
    }
}
