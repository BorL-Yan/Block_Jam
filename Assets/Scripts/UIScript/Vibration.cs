
using UnityEngine;

public class Vibration : ToggleButton
{
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
