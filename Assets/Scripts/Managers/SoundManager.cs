using System;
using System.Collections.Generic;
using Lib;
using Managers;
using UnityEngine;

public class SoundManager : SingletonScene<SoundManager>
{
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private List<SoundSettings> _sounds;


    public void PlayOneShot(SoundType soundType, float scale = 1.0f)
    {
        AudioClip clip = _sounds.Find(s => s.audioType == soundType).audioClip;
        _audioSource.PlayOneShot(clip, scale);
    }

    public void Play(SoundType soundType)
    {
        AudioClip clip = _sounds.Find(s => s.audioType == soundType).audioClip;
        _audioSource.clip = clip;
        _audioSource.Play();
    }
    
    public void PlayHello()
    {
        PlayOneShot(SoundType.Hello);
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
