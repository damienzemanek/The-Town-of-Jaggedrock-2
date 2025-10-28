using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public Transform moveLoc;
    public GameObject obj;

    public void Move() => obj.transform.position = moveLoc.position;
}
