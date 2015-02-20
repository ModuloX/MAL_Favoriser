using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkusRezai.Project.UltraGraph.ViewModel
{
    public enum ToolMode
    {
        None,
        NewVertex,
        ConnectNoneSelected,
        ConnectVertexSelected,
        ReconnectNoneSelected,
        ReconnectEdgeSelected,
        ReconnectEdgeAndVertexSelected,
        Edit,
        Delete
    }
}
