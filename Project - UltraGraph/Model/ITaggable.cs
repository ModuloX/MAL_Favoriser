using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkusRezai.Project.UltraGraph.Model
{
    interface ITaggable
    {
        List<Tag> Tags { get; }
    }
}
