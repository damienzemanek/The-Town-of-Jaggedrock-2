using UnityEngine;

public class SearchableAudioHolder : MonoBehaviour
{
    public AudioSource source;
    public AudioClip failFind;
    public AudioClip successFind;

    public GameObject effect;

    private void Awake()
    {
        effect.SetActive(false);
    }
}
