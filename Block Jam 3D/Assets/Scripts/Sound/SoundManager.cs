using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] string path;
    [SerializeField, ReadOnly] List<SO_Sound> soundInfo = new List<SO_Sound>();

    [Button]
    void LoadSoundInfo()
    {
        soundInfo.Clear();

        var objs = Resources.LoadAll(path);
        foreach (var obj in objs)
        {
            SO_Sound sound = (SO_Sound)obj;
            soundInfo.Add(sound);
        }

        soundInfo.Sort();
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
            Instance = this;
    }

    [SerializeField] List<AudioSource> sounds = new List<AudioSource>();
    [SerializeField] int soundNumber;

    public void ChangeVolume(float value)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            AudioSource sound = transform.GetChild(i).GetComponent<AudioSource>();
            sound.volume = value;
        }
    }
    [Button]
    void GenerateSounds()
    {
        if (soundNumber == 0)
            return;

        GameObject template = Instantiate(transform.GetChild(0).gameObject);
        sounds.Clear();

        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        for (int i = 0; i < soundNumber; i++)
        {
            GameObject clone = Instantiate(template, transform);
            clone.name = "Sound (" + i.ToString() + ")";
            sounds.Add(clone.GetComponent<AudioSource>());
        }

        DestroyImmediate(template);
    }


    AudioClip GetSoundInfo(SoundType soundType)
    {
        foreach (SO_Sound sound in soundInfo)
        {
            if (sound.soundType == soundType)
                return sound.clip;
        }

        return null;
    }

    public void PlaySound(SoundType soundType, float volumn = 1)
    {
        AudioClip audio = GetSoundInfo(soundType);
        if (audio == null || sounds.Count == 0)
            return;

        AudioSource sound = sounds[0];
        sounds.RemoveAt(0);
        sound.gameObject.SetActive(true);
        sound.clip = audio;
        sound.volume = volumn;
        sound.Play();
        PauseSFX(sound, audio.length);
        sound.gameObject.SetActive(true);
        StartCoroutine(PauseSFX(sound, audio.length));
    }

    IEnumerator PauseSFX(AudioSource sound, float time)
    {
        yield return new WaitForSeconds(time);
        sound.volume = 1;
        sound.Pause();
        sound.clip = null;
        sound.gameObject.SetActive(false);
        sounds.Add(sound);
    }
    public AudioSource PlayLoopSound(SoundType soundType)
    {
        AudioClip audio = GetSoundInfo(soundType);
        if (audio == null || sounds.Count == 0)
            return null;
        AudioSource sound = sounds[0];
        sounds.RemoveAt(0);
        sound.gameObject.SetActive(true);
        sound.loop = true;
        sound.clip = audio;
        sound.Play();
        return sound;
    }

    public void PauseLoopSound(AudioSource sound)
    {
        if (sound == null)
            return;
        sound.Pause();
        sound.loop = false;
        sound.clip = null;
        sound.gameObject.SetActive(false);
        sounds.Add(sound);
    }

    public void ClearSound()
    {
        sounds.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            AudioSource sound = transform.GetChild(i).GetComponent<AudioSource>();
            sound.Pause();
            sound.loop = false;
            sound.clip = null;
            sound.gameObject.SetActive(false);
            sounds.Add(sound);
        }
    }
}
