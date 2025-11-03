using UnityEngine;
using Extensions;
using System.Collections;
using ParadoxNotion.Design;
using Sirenix.OdinInspector;
using ShowIfAttribute = Sirenix.OdinInspector.ShowIfAttribute;

public class AudioPlay : MonoBehaviour
{
    [SerializeField] bool loop;
    [SerializeField] bool cutShort;
    [SerializeField] bool multi;
    bool single => !multi;
    [SerializeField] float shortTime;
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
        source.loop = loop;
        source.PlayOneShot(audio);
        if (cutShort) StartCoroutine(C_Cutshort());
    }

    public void PlayMulti(int index)
    {
        source.loop = loop;
        source.PlayOneShot(audios[index]);
        if (cutShort) StartCoroutine(C_Cutshort());
    }

    IEnumerator C_Cutshort()
    {
        yield return new WaitForSeconds(shortTime);
        Stop();
    }

    public void Stop()
    {
        source.Stop();
    }

    #region Methods
        
    #endregion

}
