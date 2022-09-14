using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    
    public Dictionary<string, Screen> screens = new Dictionary<string, Screen>();
    private GameManager gm;


    private void Start()
    {
        gm = GameManager.instance;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (!canvas.enabled)
        {
            canvas.enabled = true;
        }
        GameObject[] screenObjs = GameObject.FindGameObjectsWithTag("Screen");
        if (screenObjs.Length > 0)
        {
            foreach (GameObject obj in screenObjs)
            {
                screens.Add(obj.name.Trim(), obj.GetComponent<Screen>());
            }
        }
        else
        {
            Debug.LogError("No screens found");
        }

        
    }



    public void StopMatch(string reason)
    {
        //activates the requested screen
        switch (reason)
        {
            case "Won":
                screens["WinScreen"].ShowScreen();
                gm.playerStatus.Scoring();
                screens["WinScreen"].SendMessage("ShowScore", gm.playerStatus.scoreMatch);  

                break;
            case "Lost":
                screens["LoseScreen"].ShowScreen();
                break;
            case "Menu":
                
                break;
            case "Settings":
                
                break;
            default:
                Debug.LogError("wrong reason for stopping match");
                break;
        }
        
    }
}
