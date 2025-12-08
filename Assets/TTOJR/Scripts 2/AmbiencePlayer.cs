using UnityEngine;
using static Extensions.AudioEX;

public class AmbiencePlayer : MonoBehaviour
{
    public AudioSource source;
    public AudioClip generalAmbience;
    public AudioClip corruptingAmbience;

    public void PlayGeneralAmbience() => source.Play(generalAmbience, false).IsLooping(true);
    public void PlayCorruptingAmbience() => source.Play(corruptingAmbience, false).IsLooping(true);


}
