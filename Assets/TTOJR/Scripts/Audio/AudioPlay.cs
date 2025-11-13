using UnityEngine;
using Extensions;
using System.Collections;
using ParadoxNotion.Design;
using Sirenix.OdinInspector;
using ShowIfAttribute = Sirenix.OdinInspector.ShowIfAttribute;
using UnityEditor.Media;

public class AudioPlay : MonoBehaviour
{
    [SerializeField] bool loop;
    [SerializeField] bool cutShort;
    [SerializeField] bool fade;
    [SerializeField] bool multi;
    [SerializeField] bool enforcePlayOneShot;
    bool single => !multi;
    [SerializeField] float shortTime;
    [SerializeField] float fadeoutTime = 1f;
    #region Privates

    #endregion
    [SerializeField] AudioSource source;
    [SerializeField, ShowIf("single")] AudioClip audio;
    [SerializeField, ShowIf("multi")] AudioClip[] audios;

    private void Awake()
    {
        if (!source) source = this.TryGetOrAdd<AudioSource>();
    }
    public void Play()
    {
        this.Log("playing audio");
        source.loop = loop;

        if(!loop)
            source.PlayOneShot(audio);
        else
        {
            source.clip = audio;
            source.Play();
        }
        if (cutShort) StartCoroutine(C_Cutshort());
    }

    public void PlayRand(AudioClip[] clips)
    {
        this.Log("playing audio");
        source.loop = loop;

        if (!loop)
            source.PlayOneShot(clips.Rand());
        else
        {
            source.clip = clips.Rand();
            source.Play();
        }
        if (cutShort) StartCoroutine(C_Cutshort());
    }


    public void PlayMulti(int index)
    {
        source.loop = loop;
        if (!loop)
            source.PlayOneShot(clip: audios[index]);
        else
        {
            source.clip = audios[index];
            source.Play();
        }

        if (cutShort) StartCoroutine(C_Cutshort());
    }

    public void Play(AudioClip clip)
    {
        if (enforcePlayOneShot)
        {
            source.PlayOneShot(clip);
            if (cutShort) StartCoroutine(routine: C_Cutshort());
            return;
        }

        source.loop = loop;

        if (!loop)
            source.PlayOneShot(clip);
        else
        {
            source.clip = clip;
            source.Play();
        }

        if (cutShort) StartCoroutine(routine: C_Cutshort());
    }

    public void PlayFadeAtHalf(AudioClip clip) => PlayForSeconds(clip, clip.length, clip.length / 2);
    public void PlayFadeAtThreeQuarters(AudioClip clip) => PlayForSeconds(clip, clip.length, clip.length - (clip.length * 0.25f));


    public void PlayForSeconds(AudioClip clip, float time, float fadeAtPercent)
    {
        source.clip = clip;
        source.Play();
        StartCoroutine(C_CutshortFade(time, fadeAtPercent));
    }

    IEnumerator C_Cutshort()
    {
        yield return new WaitForSeconds(shortTime);
        Stop();
    }


    IEnumerator C_CutshortFade(float time, float startFadingAtPercent)
    {
        startFadingAtPercent = Mathf.Clamp(startFadingAtPercent, 0, 100);

        float vol = 1f;
        source.volume = vol;
        float startFadingAtSeconds = time * (startFadingAtPercent / 100f);
        float fadeOverSeconds = time - startFadingAtSeconds;
        float delay = fadeOverSeconds / 100;

        yield return new WaitForSeconds(startFadingAtSeconds);

        while (vol > 0)
        {
            yield return new WaitForSeconds(delay);
            vol -= 0.01f;
            source.volume = vol;
        }

        Stop();

        source.volume = 1f;
    }

    public void Stop()
    {
        source.Stop();
    }

    #region Methods
        
    #endregion

}
