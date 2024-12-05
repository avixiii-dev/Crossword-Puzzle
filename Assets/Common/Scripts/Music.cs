using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour
{
    public AudioSource audioSource;
    public enum Type { None, Main0, Main1, Main2 };
    public static Music instance;

    [HideInInspector]
    public AudioClip[] musicClips;

    private Type currentType = Type.None;

    private void Awake()
    {
        instance = this;
    }

    private float lastMusicTime = int.MinValue;
    public void ChangeMusic()
    {
        Type[] arr = { Type.Main1, Type.Main0, Type.Main2 };
        if (Time.time - lastMusicTime > 60)
        {
            Play(CUtils.GetRandom(arr));
            lastMusicTime = Time.time;
        }
        else
        {
            Play();
        }
    }

    public bool IsMuted()
    {
        return !IsEnabled();
    }

    public bool IsEnabled()
    {
        return CPlayerPrefs.GetBool("music_enabled", true);
    }

    public void SetEnabled(bool enabled, bool updateMusic = false)
    {
        CPlayerPrefs.SetBool("music_enabled", enabled);
        if (updateMusic)
            UpdateSetting();
    }

    public void Play(Music.Type type)
    {
        if (type == Type.None) return;
        if (currentType != type || !audioSource.isPlaying)
        {
            StartCoroutine(PlayNewMusic(type));
        }
    }

    public void Play()
    {
        Play(currentType);
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    private IEnumerator PlayNewMusic(Type type)
    {
        while (audioSource.volume >= 0.1f)
        {
            audioSource.volume -= 0.2f;
            yield return new WaitForSeconds(0.1f);
        }
        audioSource.Stop();
        currentType = type;
        audioSource.clip = musicClips[(int)type];
        if (IsEnabled())
        {
            audioSource.Play();
        }
        audioSource.volume = 1;
    }

    private void UpdateSetting()
    {
        if (audioSource == null) return;
        if (IsEnabled())
            Play();
        else
            audioSource.Stop();
    }
}
