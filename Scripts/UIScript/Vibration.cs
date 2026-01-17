
using UnityEngine;

public class Vibration : ToggleButton
{
    protected override void Apply()
    {
        base.Apply();
        string active = isOn ? "Active" : "Deactive" ;
        Debug.Log($"Vibration: {active}");
    }

    public override void SaveSettings()
    {
        var settings = GameSave.GetSettings();
        settings.Vibration = isOn;
        GameSave.SetSettings(settings);
        GameSave.Save();
    }

    public override  void LoadSettings()
    {
        var settings = GameSave.GetSettings();
        isOn = settings.Vibration;
    }
}
