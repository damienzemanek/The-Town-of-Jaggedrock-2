using UnityEngine;
using Extensions;
using System.Collections;

public class AudioPlay : MonoBehaviour
{
    [SerializeField] bool loop;
    [SerializeField] bool cutShort;
    [SerializeField] float shortTime;
    #region Privates

    #endregion
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip audio;

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
