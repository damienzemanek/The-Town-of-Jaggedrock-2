using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChange : MonoBehaviour
{

    #region Privates
        
    #endregion

    public void ChangeScene(int num)
    {
        SceneManager.LoadScene(num);
    }


    #region Methods
        
    #endregion

}
