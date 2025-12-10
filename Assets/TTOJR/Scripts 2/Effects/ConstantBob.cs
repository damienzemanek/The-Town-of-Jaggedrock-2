using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

public class ConstantBob : MonoBehaviour
{
    [SerializeField] Deviatable upAmount;
    [SerializeField] Vector2 moveTime;

    Vector3 origPos;
    Vector3 newPos;

    private void Awake()
    {
        origPos = transform.position;
        newPos = new Vector3(origPos.x, origPos.y + upAmount.value, origPos.z);
    }

    private void Start()
    {
        Up();
    }

    void Up()
    {
        StopAllCoroutines();
        transform.Lerp(newPos, moveTime.Rand(), this, Down);
    }

    void Down()
    {
        StopAllCoroutines();
        transform.Lerp(origPos, moveTime.Rand(), this, Up);
    }
}
