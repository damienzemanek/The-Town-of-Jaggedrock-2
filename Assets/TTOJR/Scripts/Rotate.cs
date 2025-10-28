using System.Collections;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public GameObject obj;
    public float delay = 0.02f;
    private void OnEnable()
    {
        StartCoroutine(DoRotate());
    }

    private void OnDisable()
    {
        StopCoroutine(DoRotate());
    }

    IEnumerator DoRotate()
    {
        while (true)
        {
            obj.transform.Rotate(0, 0, 1);
            yield return new WaitForSeconds(delay);
        }
    }
}
