using System.Collections.Generic;
using System.Linq;

namespace FancyLines.Lines.InfoTypes
{
    public interface INormalInfoProvider
    {
        public List<INormalInfo> Infos { get; }
        public List<string> Names => Infos.Select(n => $"{Infos.IndexOf(n):00} - " + n.Name).ToList();
    }
}