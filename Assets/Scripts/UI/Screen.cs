using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void ScreenAction();

public abstract class Screen : MonoBehaviour
{
    public GameObject myScreen;
    protected GameManager gm;
    public Dictionary<string, ScreenButton> myButtons = new Dictionary<string, ScreenButton>();



    // Start is called before the first frame update
    protected virtual void Start()
    {
        gm = GameManager.instance;
        myScreen = transform.GetChild(0).gameObject;
        GetButtons();
        StartCoroutine(AfterStart());
    }

    protected virtual IEnumerator AfterStart()
    {
        yield return new WaitForFixedUpdate();
        ShowScreen(false);
        
    }
    
    //define an action for a ScreenButton
    public void ConfigButton(ScreenButton button, string methodName)
    {
        button.myAction = methodName;
        button.screen = this;
    }

    //find reference of ScreenButton component in children and add to dictionary
    protected void GetButtons()
    {
        ScreenButton[] buttons = gameObject.GetComponentsInChildren<ScreenButton>();
        if (buttons.Length > 0)
        {
            foreach (ScreenButton btn in buttons)
            {
                myButtons.Add(btn.gameObject.name.Trim(), btn);
            }
        }
        else
        {
            Debug.LogWarning("no ScreenButtons found in children");
        }
    }

    //shows this or a target screem, acording to parameters
    public virtual void ShowScreen(bool show = true, Screen targetScreen = null)
    {
        if (targetScreen == null)
        {
            myScreen.SetActive(show);
        }
        else
        {
            targetScreen.myScreen.SetActive(show);
        }
    }


}
