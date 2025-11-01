using UnityEngine;
using DependencyInjection;

public class Referencer : MonoBehaviour, IDependencyProvider
{

    #region Privates
    [Provide] Referencer Provide() => this;
    #endregion

    public GameObject frostEffect;


    #region Methods
        
    #endregion

}
