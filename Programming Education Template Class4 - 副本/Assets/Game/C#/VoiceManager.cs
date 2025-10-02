using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum VoiceType
{
    Move,
    Attack,
    Hit,
    Collect,
    Place,
    UIShow,
    Successful,
    Fail,
}

public class VoiceManager : MonoBehaviour
{
    public static VoiceManager Instance;
    public GameObject voicePrefab;
    public List<Voice> voices;
    public AudioSource audioSource;
    public List<Voice> backgroundVoices;

    private float volumePartic;
    private float volumeMusic;
    private Button[] allButtons;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        volumePartic = 1f;
        volumeMusic = 0.3f;
    }

    private void Start()
    {
       // DontDestroyOnLoad(gameObject);
        //SwitchBackground(0);
    }

    public void SetVolumePartic(float volume)
    {
        volumePartic = volume;
    }

    public void SetVolumeMusic(float volume)
    {
        volumeMusic = volume;
        audioSource.volume = volumeMusic;
    }

    public void Play(VoiceType type, bool isInstate)
    {
        if (isInstate)
        {
            if (voices.Find(x => x.type == type) != null)
            {
                GameObject go = Instantiate(voicePrefab);
                go.GetComponent<AudioSource>().clip = voices.Find(x => x.type == type).clip;
                go.GetComponent<AudioSource>().Play();
                go.GetComponent<AudioSource>().volume = volumePartic;
                Destroy(go, 2f);
            }
        }
    }

    public void SwitchBackground(int value)
    {
         // audioSource.clip=backgroundVoices[value].clip;
         // audioSource.Play();
    }
}

[Serializable]
public class Voice
{
    public VoiceType type;
    public AudioClip clip;
}