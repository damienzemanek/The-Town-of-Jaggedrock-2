using Sirenix.OdinInspector;
using UnityEngine;

public class Raycaster : MonoBehaviour
{
    [field: ShowInInspector] [field:SerializeField] public float dist { get; private set; }
    [field: ShowInInspector] [field: SerializeField] public Transform startPoint { get; private set; }
    public bool Raycast(out RaycastHit hit, LayerMask mask)
    {
        Ray ray = new Ray(startPoint.position, startPoint.transform.forward);
        if(Physics.Raycast(ray, out hit, dist, mask))
        {
            Debug.DrawLine(ray.origin, end: hit.point, Color.magenta);
            return true;
        }
        return false;
    }
}
