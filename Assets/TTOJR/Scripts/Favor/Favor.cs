using Sirenix.OdinInspector;
using UnityEngine;
using NodeCanvas;
using ParadoxNotion.Design;

[CreateAssetMenu(fileName = "New Favor", menuName = "ScriptableObjects/Favor")]
public class SO_Favor : ScriptableObject
{
    int minorFavorGain = 1;
    int majorFavorGain = 3;

    [SerializeField, HideInInspector, ExposeField] int _favor;
    [ShowInInspector] public int favor
    {
        get => _favor;
        set
        {
            int clamped = Mathf.Clamp(value, -10, 10);
            _favor = clamped;
        }
    }

    public enum FavorStatus
    {
        Hated,
        Unliked,
        Neutral,
        Liked,
        Friend
    }

    [ShowInInspector] public FavorStatus status
    {
        get
        {
            int clampedFavor = Mathf.Clamp(favor, -10, 10);
            if (clampedFavor <= -6) return FavorStatus.Hated;
            if (clampedFavor <= -2) return FavorStatus.Unliked;
            if (clampedFavor <= 1) return FavorStatus.Neutral;
            if (clampedFavor <= 6) return FavorStatus.Liked;
            return FavorStatus.Friend;
        }
    }


    public void GainMinorFavor() => favor += minorFavorGain;
    public void GainMajorFavor() => favor += majorFavorGain;
    public void LoseMinorFavor() => favor -= minorFavorGain;
    public void LoseMajorFavor() => favor -= majorFavorGain;

}
