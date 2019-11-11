using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMManager : MonoBehaviour
{
    static public BGMManager instance;

    public AudioClip[] clips;
    private AudioSource source;
    private WaitForSeconds waitTime;


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
       // Start is called before the first frame update


    void Start()
    {
        source = this.GetComponent<AudioSource>();

        source.volume = DatabaseManager.instance.VolumeSettingValue;

        waitTime = new WaitForSeconds(0.01f);
    }

    public void SetVolume(float vol)
    {
        source.volume = vol;
        DatabaseManager.instance.VolumeSettingValue = vol;
    }
    public float GetVolumn()
    {
        return source.volume;
    }
    public void Play(int _playMusicTrack)
    {
        source.clip = clips[_playMusicTrack];
        source.volume = source.volume;

        source.Play();
    }

    public void Pause()
    {
        source.Pause();
    }
    public void UnPause()
    {
        source.UnPause();
    }

    public void Stop()
    {
        source.Stop();
    }
    public void FadeOutMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutMusicCor());
    }
    IEnumerator FadeOutMusicCor()
    {
        float i = DatabaseManager.instance.VolumeSettingValue;
        while (true)
        {
            source.volume = i;

            i -= 0.01f;

            yield return waitTime;

            if (i <= 0)
                break;
        }
    }
    public void FadeInMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInMusicCor());
    }
    IEnumerator FadeInMusicCor()
    {
        float i = 0;

        while (true)
        {
            source.volume = i;

            i += 0.01f;

            yield return waitTime;
            if (i >= DatabaseManager.instance.VolumeSettingValue)
                break;
        }
    }
}
