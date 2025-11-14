using DependencyInjection;
using Sirenix.OdinInspector;
using UnityEngine;

public class Attacked : MonoBehaviour
{

    #region Privates
    [SerializeField] Animator animator;
    [SerializeField] string animName;
    [Inject] EntityControls controls;
    [SerializeField] Look look;
    [SerializeField] AudioPlay sound;
    [SerializeField] AudioClip deathSound;
    #endregion


    [Button]
    public void Die()
    {
        animator.Play(animName);
        controls.canMove = false;
        look.ToggleUpdateMouseLooking(false);
        sound.PlayFadeAtHalf(deathSound);
    }

    #region Methods
        
    #endregion

}
