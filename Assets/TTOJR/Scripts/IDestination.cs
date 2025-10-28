using UnityEngine;

public interface IDestination 
{
    public abstract bool preventContact { get; set; }
    public abstract void MakeContact();
    public abstract void ResetContact();
}
