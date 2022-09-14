using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    AsyncOperation async;
    Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        Debug.Log("myTest:"+"this canvas is "+canvas);
    }

    public void AfterLoad(string loadOrigin)
    {
        if (SaveSystem.instance.cloud.authenticated && loadOrigin == "Local")
        {
            return;
        }

        if (CheckIfTutorial())
        {
            ShowLoadScreen(1);
        }
        else
        {
            ShowLoadScreen(2);
        }
    }

    public bool CheckIfTutorial()
    {
        PlayerStatus pS = SaveSystem.instance.playerStatus;
        if (pS.currentEnergy + pS.spentEnergy > 0)
        {
            return false;
        }
        return true;
    }

    public void ShowLoadScreen(int sceneToLoad)
    {
        StartCoroutine(LoadSceneAssync(sceneToLoad));
    }

    IEnumerator LoadSceneAssync(int sceneToLoad)
    {
        Debug.Log("myTest:"+"scene to load: " + sceneToLoad);

        canvas.enabled = true;

        int timeout = 0;
        while(SaveSystem.instance.isDownloading&&timeout < 10000)
        {
            yield return new WaitForFixedUpdate();
            timeout++;
        }
        if (timeout >= 1000)
        {
            Debug.LogError("loadscreen timeout");
            yield break;
        }
        
        //starts loading scene 1 on the background
        async = SceneManager.LoadSceneAsync(sceneToLoad);

        //prevents next scene to start and this scene to be unloaded
        async.allowSceneActivation = false;

        
        //will repeatd untill the scene is ready to load
        while (async.isDone == false)
        {
            //Debug.Log("myTest:"+"2: "+ async.isDone);
            //if progress is big enough, allow activation
            if (async.progress >= 0.9f)
            {

                async.allowSceneActivation = true;
                canvas.enabled = false;
                yield break;
            }
            //Debug.Log("myTest:"+"3: " + async.progress);
            yield return null;
        }
        async.allowSceneActivation = true;
        canvas.enabled = false;
        
        //Debug.Log("myTest:"+"4: " + async.isDone);
        
    }
}
