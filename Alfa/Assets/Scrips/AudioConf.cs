using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioConf
{
    public string name;
    public AudioResourceType audioType;
    public AudioClip clip;

    public AudioConf(string name, AudioResourceType audioType, AudioClip clip)
    {
        this.name = name;
        this.audioType = audioType;
        this.clip = clip;
    }
}