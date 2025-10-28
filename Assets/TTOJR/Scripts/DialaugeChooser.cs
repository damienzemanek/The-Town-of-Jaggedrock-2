using System;
using UnityEngine;
using UnityEngine.Rendering;
using static DialaugeChooser;
using Extensions;

public class DialaugeChooser : MonoBehaviour
{
    public enum DialaugeChoice
    {
        DISLIKED,
        NEUTRAL,
        IMPORTANT,
        SECRET
    }

    [Serializable]
    public class ChanceRoll
    {
        public SO_Favor.FavorStatus favorLevel;
        public float disliked;
        public float neutral;
        public float important;
        public float secret;

        public ChanceRoll(float _disliked, float _neutral, float _important, float _secret)
        {
            disliked = _disliked;
            neutral = _neutral;
            important = _important;
            secret = _secret;
        }

        public DialaugeChoice DetermineChoice()
        {
            float total = disliked + neutral + important + secret;
            if (total <= 0f)
                return DialaugeChoice.NEUTRAL;

            float roll = UnityEngine.Random.Range(0f, total);
            float current = 0f;

            if ((current += disliked) >= roll) return DialaugeChoice.DISLIKED;
            if ((current += neutral) >= roll) return DialaugeChoice.NEUTRAL;
            if ((current += important) >= roll) return DialaugeChoice.IMPORTANT;
            return DialaugeChoice.SECRET;
        }
    }


    Dialuage dialuage;



    public DialaugeChoice dialaugeChoice;
    public ChanceRoll[] rolls = new ChanceRoll[5];

    //new ChanceRoll(_disliked: 60, _neutral: 30, _important: 10, _secret: 0), //Hated
    //    new ChanceRoll(_disliked: 40, _neutral: 40, _important: 20, _secret: 0), //Disliked
    //    new ChanceRoll(_disliked: 0, _neutral: 70, _important: 30, _secret: 0), //Neutral
    //    new ChanceRoll(_disliked: 0, _neutral: 60, _important: 40, _secret: 0), //Liked
    //    new ChanceRoll(_disliked: 0, _neutral: 50, _important: 40, _secret: 10) //Friend

    private void Awake()
    {
        dialuage = this.TryGet<Dialuage>();
    }


    public void SetDialaugeChoice()
    {
        this.Log($"favor is {dialuage.favor.status}");
        ChanceRoll dialaugeOptions = rolls[(int)dialuage.favor.status];
        dialaugeChoice = dialaugeOptions.DetermineChoice();
        this.Log($"The roll was {dialaugeChoice}");
    }
}
