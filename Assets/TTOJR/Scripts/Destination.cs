using UnityEngine;
using UnityEngine.Events;

public class Destination : MonoBehaviour, IDestination
{
    [field: SerializeField] public bool preventContact { get; set; } = false;
    public void MakeContact() { Contact?.Invoke(); Debug.Log("Destination: Made Contact"); }
    public void ResetContact() => Reset?.Invoke();

    public UnityEvent Contact;
    public UnityEvent Reset;

    private void Start()
    {
        Reset?.Invoke();
    }

}
