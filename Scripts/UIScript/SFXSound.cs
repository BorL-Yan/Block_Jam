using UnityEngine;
using UnityEngine.Audio;

public class SFXSound : ToggleButton
{
    [SerializeField] protected AudioMixer mainAudioMixer;
    
    public override void LoadSettings()
    {
        var settings = GameSave.GetSettings();
        isOn = settings.SFX;
    }
    public override void SaveSettings()
    {
        var settings = GameSave.GetSettings();
        settings.SFX = isOn;
        GameSave.SetSettings(settings);
        GameSave.Save();
    }
    
    protected override void Apply()
    {
        base.Apply();
        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat("SFXVolume", isOn ? 0f : -80f);
        }
    }
}