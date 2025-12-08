using UnityEngine;
using DependencyInjection;

public class Referencer : MonoBehaviour, IDependencyProvider
{

    #region Privates
    [Provide] Referencer Provide() => this;
    #endregion

    public GameObject frostEffect;
    public FadeIn frostScreen;
    public AudioPlay pickupPlayer;


    #region Methods
        
    #endregion

}
