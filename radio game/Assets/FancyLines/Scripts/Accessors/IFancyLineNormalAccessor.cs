using FancyLines.Lines.InfoTypes;
using System.Collections.Generic;

public interface IFancyLineNormalAccessor
{
    public INormalInfoProvider Provider { get; }
    public List<string> NormalOptions { get; }

    public T GetNormal<T>(string _name) where T : INormalInfo
    {
        int index = int.Parse(_name[..2]);

        return (T) Provider.Infos[index];
    }
}
