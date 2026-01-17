using System;
using System.Collections.Generic;
using UnityEngine;

public class TrailController : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private List<TrailColor> _trailColors;

    public void Activate(bool value)
    {
        gameObject.SetActive(value);
        _trailRenderer.Clear();
    }

    public void Init(BlockColor blockColor)
    {
        _trailRenderer.colorGradient = _trailColors.Find(c => c.BlockColor == blockColor).Color;
    }
    
}

[Serializable]
public class TrailColor
{
    public BlockColor BlockColor;
    public Gradient Color;
}
