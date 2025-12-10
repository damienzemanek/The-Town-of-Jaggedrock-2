using UnityEngine;

public class EffigySearchable : MonoBehaviour
{
    public GameObject prefab;

    private void OnEnable()
    {
        Instantiate(prefab, transform);
    }
}
