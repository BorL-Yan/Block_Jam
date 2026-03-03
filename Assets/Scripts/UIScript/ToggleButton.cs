using System;
using UnityEngine;

public abstract class ToggleButton : UIButton, IToggle, ISettings
{
    public bool isOn { get; set; }
    [SerializeField] protected GameObject _closeIcon;

    private void Start()
    {
        LoadSettings();
        Apply();
    }

    protected override void Click()
    {
        Toggle();
    }

    public virtual void Toggle()    
    {
        isOn = !isOn;
        _closeIcon.SetActive(!isOn);
        Apply();
        SaveSettings();
    }

    protected virtual void Apply()
    {
        _closeIcon.SetActive(!isOn);
    }
    
    public abstract void SaveSettings();
    public abstract void LoadSettings();
}
