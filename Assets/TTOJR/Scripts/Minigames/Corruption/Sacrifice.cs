using System.Linq;
using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Sacrifice : MonoBehaviour
{

    #region Privates
    [SerializeField] bool complete;
    #endregion
    public int[] generatedNumbers = new int[5];
    public SacrificeCandle[] correctPlacements;

    public TextMeshPro mainNumbersText;
    [SerializeField] int currentCandle;
    [SerializeField] int correctInOrderCandles;
    public UnityEvent failedHook;
    public UnityEvent stoppedHook;

    private void OnEnable()
    {
        if(failedHook == null) failedHook = new UnityEvent();
        if(stoppedHook == null) stoppedHook = new UnityEvent();

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
            this.DelayedCall(StopSacrifice, 3);

        if (correctInOrderCandles < 5 && currentCandle >= 5)
            this.DelayedCall(ResetSacrifice, 3);
    }

    public void StopSacrifice()
    {
        complete = true;
        stoppedHook?.Invoke();
    }

    public void ResetSacrifice()
    {
        OnEnable();
        failedHook?.Invoke();
    }


    #region Methods
        
    #endregion

}
