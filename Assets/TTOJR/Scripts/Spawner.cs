using UnityEngine;
using Sirenix.OdinInspector;
using Extensions;
using System.Linq;

public class Spawner : MonoBehaviour
{
    [BoxGroup("Spawn Choice")] public bool single;
    [BoxGroup("Spawn Choice")] public bool multi;
    [ShowIf("multi")] public bool multiSpawnAllAtOnce;
    [ShowIf("multi")] public bool multiSpawnSelectSingle;

    [ShowIf("single")] public GameObject prefab;
    [ShowIf("multi")] public GameObject[] prefabs;
    public Transform location;

    public void Spawn(int index = 0)
    {
        if (multi)
            if (!multiSpawnAllAtOnce && !multiSpawnSelectSingle)
                this.Error("Please select one of the multi spawn options");

        if (single) SingleSpawn();
        if (multiSpawnAllAtOnce) SpawnAllAtOnce();
        if (multiSpawnSelectSingle) SpawnSelect(index);
    }

    void SingleSpawn()
    {
        Instantiate(
        prefab,
        location.position,
        Quaternion.identity,
        null
        );
    }

    void SpawnAllAtOnce()
    {
        prefabs.ToList().ForEach(p => Instantiate(
            p,
            location.position,
            Quaternion.identity,
            null
            ));
    }

    void SpawnSelect(int index)
    {
        Instantiate(
            prefabs[index],
            location.position,
            Quaternion.identity,
            null
        );
    }
}
