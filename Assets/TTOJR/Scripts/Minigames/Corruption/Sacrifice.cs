using Extensions;
using UnityEngine;

public class Sacrifice : MonoBehaviour
{

    #region Privates

    #endregion
    public int[] generatedNumbers = new int[5];
    public SacrificeCandle[] correctPlacements;

    private void OnEnable()
    {
        GenerateNumbers();
    }

    public void GenerateNumbers()
    {
        Vector2 randomNumBetween0and9 = new Vector2(0, 9); 

        for (int i = 0; i < generatedNumbers.Length; i++)
        {
            int num = (int)randomNumBetween0and9.Rand();
            num = Mathf.Clamp(num, 0, 9);

            generatedNumbers[i] = num;
        }
    }

    public void ApplyCandleNumbers()
    {

    }


    #region Methods
        
    #endregion

}
