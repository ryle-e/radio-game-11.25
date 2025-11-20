using FancyLines.Lines;
using FancyLines.Lines.InfoTypes;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public abstract class OscillatorAnimator : MonoBehaviour, IFancyLineNormalAccessor
{
    [SerializeField] private OscillatorFancyLine provider;

    [SerializeField, Dropdown("NormalOptions")] private string selectedNormal;

    protected OscillatorNormal normal;


    public INormalInfoProvider Provider => provider;
    public List<string> NormalOptions => Provider != null ? Provider.Names : new() { "Provider not selected!" };

    private void Start()
    {
        normal = ((IFancyLineNormalAccessor)this).GetNormal<OscillatorNormal>(selectedNormal);

        OnStart();
    }

    protected virtual void OnStart() { }
}
