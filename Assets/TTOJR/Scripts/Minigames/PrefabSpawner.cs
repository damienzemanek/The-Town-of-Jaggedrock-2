using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefab;
    public Transform spwawnLoc;
    public void SpawnPrefab()
    {
        Instantiate(
            prefab,
            spwawnLoc.position,
            Quaternion.identity,
            spwawnLoc.transform.parent
            );
    }
}
