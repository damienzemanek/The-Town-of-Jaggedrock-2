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

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    #region Methods

    #endregion

}
