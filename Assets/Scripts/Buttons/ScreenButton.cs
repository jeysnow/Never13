using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenButton : ButtonAction
{
    
    public string myAction;
    public Screen screen;
    public TMP_Text text;
    //creating a delegate variable to store target 

    protected override void Start()
    {
        base.Start();
        text = GetComponentInChildren<TMP_Text>();
    }


    public override void Action()
    {

        
        //make sure this button was configured;
        if(myAction == null)
        {
            Debug.LogError("no action configured for button " + this.gameObject.name);
            return;
        }
        screen.SendMessage(myAction);
    }

    //method for whowing or hiding this parent's or target screen
    
}
