using System.Collections;
using UnityEngine;


public enum AudioResourceType
{
    Music,
    Effect
}

public class AudioController : MonoBehaviour
{
    public AudioSource musicSource;
    private string currentMusic;
    public string getCurrentMusic()
    {
        return currentMusic;
    }

    public void setVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void PlayAudio(AudioConf audioConf)
    {
        if (audioConf.audioType == AudioResourceType.Music)
        {
            PlayMusic(audioConf);
            currentMusic = audioConf.name;
        }
        else if (audioConf.audioType == AudioResourceType.Effect)
        {
            StartCoroutine(PlayEffect(audioConf));           
        }
    }

    private void PlayMusic(AudioConf audioConf)
    {
        musicSource.clip = audioConf.clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    private IEnumerator PlayEffect(AudioConf audioConf)
    {

        AudioSource source = new GameObject("effectSource").AddComponent<AudioSource>();
        source.volume = musicSource.volume;
        source.gameObject.transform.SetParent(musicSource.transform, false);
        source.PlayOneShot(audioConf.clip);
        while (source.isPlaying)
        {
            yield return null;
        }
        Destroy(source);
    }
}
