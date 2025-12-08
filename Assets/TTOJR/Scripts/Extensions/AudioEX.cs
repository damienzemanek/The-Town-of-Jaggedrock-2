using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Extensions
{
    public static class AudioEX
    {

        public static AudioSource Play(this AudioSource source, AudioClip clip, bool oneShot = true)
        {
            if (oneShot)
                source.PlayOneShot(clip);
            else
            {
                source.clip = clip;
                source.Play();
            }

            return source;
        }

        public static AudioSource IsLooping(this AudioSource source, bool val)
        {
            source.loop = val;
            return source;
        }

        public static AudioSource CutShort(this AudioSource source, float pct)
        {
            if (source.clip == null) return source;
            pct = Mathf.Clamp01(pct);

            float cutTime = source.clip.length * pct;

            source.time = 0f;
            source.Play();

            source.SetScheduledEndTime(AudioSettings.dspTime + cutTime);

            return source;
        }

        public static AudioSource PlaySimultanious(this AudioSource source, AudioClip[] clips)
        {
            foreach (AudioClip clip in clips)
                source.PlayOneShot(clip);
            return source;
        }

        public static AudioSource PlaySimultanious(this AudioSource source, List<AudioClip> clips)
        {
            foreach (AudioClip clip in clips)
                source.PlayOneShot(clip);
            return source;
        }



        public static void PlayForSeconds(this MonoBehaviour host, AudioSource source, AudioClip clip, float time, float fadeAtPercent)
        {
            source.clip = clip;
            source.Play();
            host.StartCoroutine(C_CutshortFade(source, time, fadeAtPercent));
        }

        public static IEnumerator C_CutshortFade(AudioSource source, float time, float startFadingAtPercent)
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

            source.Stop();
        }


    }
}