using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using DesignPatterns.CreationalPatterns;
using static Extensions.FadeEX;


public class LoadScene : Singleton<LoadScene>
{
    [SerializeField] float minTimeLoading = 1f;
    [SerializeField] FadeSettings fade;

    private void OnDisable()
    {
        fade.SetAlpha(0);
        print("a");
    }

    public void LoadSceneFadeScreenToOpaque(int num)
    {
        StartCoroutine(C_FadeToOpaque(fade, 
            () => StartCoroutine(Load(num))));
    }

    public void LoadImmediate(int num) => StartCoroutine(Load(num));

    public GameObject[] LoadingScreenObjects;
    public GameObject[] disables;


    IEnumerator Load(int sceneId)
    {
        LoadingScreenObjects.SetAllActive(true);
        disables.SetAllActive(false);
        yield return new WaitForSeconds(minTimeLoading);
        StartCoroutine(LoadSceneAsync(sceneId));
    }
    IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        while(!operation.isDone)
        {
            yield return null;
        }

        fade.SetAlpha(0);
    }
}
