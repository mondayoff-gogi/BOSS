using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;
    private AudioSource source;

    public float Volumn;
    public bool loop;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.loop = loop;
        source.volume = Volumn;
    }

    public void Play()
    {
        source.Play();
    }
    public void Stop()
    {
        source.Stop();
    }
    public void SetLoop()
    {
        source.loop = true;
    }
    public void SetLoopCancel()
    {
        source.loop = false;
    }

    public void SetVolumn()
    {
        source.volume = Volumn;
    }
}

public class SoundManager : MonoBehaviour
{
    static public SoundManager instance;

    [SerializeField]
    public Sound[] sounds;

    private void Awake()
    {        
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
    }  //--------------인스턴스화를 위함 ----

    void Start()
    {
        for(int i=0;i<sounds.Length;i++)
        {
            GameObject soundObject = new GameObject(sounds[i].name);
            sounds[i].SetSource(soundObject.AddComponent<AudioSource>());
            soundObject.transform.SetParent(this.transform);
        }
    }

    public void Play(int i)
    {
        sounds[i].Play();
    }
    public void Stop(int i)
    {
        sounds[i].Stop();
    }
    public void SetLoop(int i)
    {
        sounds[i].SetLoop();
    }
    public void SetLoopCancel(int i)
    {
        sounds[i].SetLoopCancel();
    }
    public void SetVolumn(int i,float vol)
    {
        sounds[i].Volumn = vol;
        sounds[i].SetVolumn();
    }
}
