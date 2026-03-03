using System;
using System.Collections.Generic;
using Lib;
using Managers;
using UnityEngine;

public class SoundManager : SingletonScene<SoundManager>
{
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private List<SoundSettings> _sounds;


    public void PlayOneShot(SoundType soundType)
    {
        AudioClip clip = _sounds.Find(s => s.audioType == soundType).audioClip;
        _audioSource.PlayOneShot(clip);
    }

    public void PlayOneShot(SoundType soundType, float scale)
    {
        AudioClip clip = _sounds.Find(s => s.audioType == soundType).audioClip;
        _audioSource.PlayOneShot(clip, scale);
    }
}

[Serializable]
public class SoundSettings
{
    public AudioClip audioClip;
    public SoundType audioType;
}


public enum SoundType
{
    Activate,
    ActivateInList,
    BoxBreak,
    Hello,
    Jump,
    Merge,
    Walk,
    Win,
}
