using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField][Range(0f, 1f)] private float soundEffectVolume;
    [SerializeField][Range(0f, 1f)] private float musicVolume;

    Pooling objectPool;
    AudioSource musicAudioSource;
    public AudioClip clip;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }


        musicAudioSource = GetComponent<AudioSource>();
        musicAudioSource.clip = clip;
        musicAudioSource.volume = musicVolume;
        musicAudioSource.loop = true;
        musicAudioSource.Play();

        objectPool = GetComponent<Pooling>();
        objectPool.CreatePool(transform);
    }


    public void ChangeBackGroundMusic(AudioClip clip, float soundVolume)
    {
        musicAudioSource.Stop();
        musicAudioSource.clip = clip;
        musicAudioSource.volume = soundVolume;
        musicAudioSource.Play();

    }
    public void PlayClip(AudioClip clip, float volume)
    {
        GameObject obj = objectPool.GetPoolItem("SoundSource");
        obj.SetActive(true);
        SoundSource soundSource = obj.GetComponent<SoundSource>();
        soundSource.Play(clip, volume);
    }
    public void PlayClip(AudioClip clip)
    {
        GameObject obj = objectPool.GetPoolItem("SoundSource");
        obj.SetActive(true);
        SoundSource soundSource = obj.GetComponent<SoundSource>();
        soundSource.Play(clip, soundEffectVolume);
    }

}
