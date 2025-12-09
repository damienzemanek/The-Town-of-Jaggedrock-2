using UnityEngine;
using Extensions;
using static Extensions.TransformEX;
using static TransformUtility;

public class UISelect : MonoBehaviour
{
    public int current;
    public int amountOfSelects;
    public GameObject TextObj;
    public float moveAmount;
    public float speed;

    public void LeftorSubtract()
    {
        if (current <= 0) return;
        current--;

        Vector3 v = TextObj.transform.position;
        v = v.With(x: v.x + moveAmount);
        TextObj.transform.Lerp(v, speed, this);
    }

    public void RightorAdd()
    {
        if(current >= amountOfSelects) return;

        current++;

        Vector3 v = TextObj.transform.position;
        v = v.With(x: v.x - moveAmount);
        TextObj.transform.Lerp(v, speed, this);
    }
}
