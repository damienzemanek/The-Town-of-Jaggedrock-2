using System.Linq;
using DependencyInjection;
using Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Sacrifice : RuntimeInjectableMonoBehaviour
{

    #region Privates
    [SerializeField] bool complete;
    #endregion
    public int[] generatedNumbers = new int[5];
    public SacrificeCandle[] correctPlacements;

    public TextMeshPro mainNumbersText;
    [SerializeField] int currentCandle;
    [SerializeField] int correctInOrderCandles;

    [TabGroup("Audio")] public AudioSource source;
    [TabGroup("Audio")] public AudioClip[] failAudsSimul;

    public CorruptonLocation loc;




    private void OnEnable()
    {
        
        currentCandle = 0;
        correctInOrderCandles = 0;
        GenerateNumbers();
        ApplyMainNumbers();
        GiveNumbersToCandles();

    }


    void GenerateNumbers()
    {
        Vector2 randomNumBetween0and9 = new Vector2(0, 9); 

        for (int i = 0; i < generatedNumbers.Length; i++)
        {
            int num = (int)randomNumBetween0and9.Rand();
            num = Mathf.Clamp(num, 0, 9);

            generatedNumbers[i] = num;
        }
    }

    void ApplyMainNumbers() 
    {
        string concatNum = "";
        for(int i = 0; i < generatedNumbers.Length; i++)
        {
            concatNum = concatNum + "" + generatedNumbers[i];
        }
        mainNumbersText.text = "" + concatNum;
    }

    void GiveNumbersToCandles()
    {
        for(int i = 0; i <  correctPlacements.Length; i++)
            correctPlacements[i].InitializeCandle(generatedNumbers[i], this);
    }

    public void AttemptToBlowout(int num)
    {
        this.Log($"Comparing {num} and {generatedNumbers[currentCandle]}");

        if (num == generatedNumbers[currentCandle])
            correctInOrderCandles++;

        currentCandle++;


        if (correctInOrderCandles == 5 && currentCandle >= 5)
            this.DelayedCall(StopSacrifice, 2);

        if (correctInOrderCandles < 5 && currentCandle >= 5)
            this.DelayedCall(ResetSacrifice, 2);
    }

    public void StopSacrifice()
    {
        complete = true;
        loc.haltedHook?.Invoke();
        this.DelayedCall(() => Destroy(gameObject), 5);
    }

    public void ResetSacrifice()
    {
        OnEnable();
        source.PlaySimultanious(failAudsSimul);
    }

    public Sacrifice SetLoc(CorruptonLocation _loc)
    {
        loc = _loc;
        return this;
    }

    public void SelfDestroy()
    {
        Destroy(gameObject, 3f);
    }


    #region Methods

    #endregion

}
