using System.Collections.Generic;


namespace ILDAViewer.net.interfaces
{
    internal interface IVertexes
    {
        IEnumerable<float> Vertexes { get; }
        IEnumerable<uint> LinesIndex { get; }
    }
}
