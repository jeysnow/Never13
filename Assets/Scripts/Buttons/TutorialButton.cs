using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButton : ButtonAction
{
    new Tutorial gm;
    private int currentTip;
    protected override void Start()
    {
        base.Start();
        gm = GameManager.instance as Tutorial;
        gm.confirmButton = this;
        ShowButton(false);
    }
    public override void Action()
    {
        if (TipAction())
        {
            BaseAction();
        }
        
    }

    //executes actions specific for current tip
    private bool TipAction()
    {
        if(currentTip<0 ||currentTip >= gm.instructions.Length)
        {
            Debug.LogError("Wrong currentTip for TipAction");
        }
        switch (currentTip)
        {
            case 0:
                gm.dice["Yellow"].dieButton.Action();
                break;
            case 3:
                gm.UndoLastMove();
                gm.ShowTip(4);
                return false;
            default:
                //some tips don't need special actions
                break;
        }
        return true;
    }


    //goes back to regular tutorial play
    private void BaseAction()
    {
        StartCoroutine(gm.WaitToTip());
        gm.highLightScreen.gameObject.SetActive(false);
        ShowButton(false);
        
    }

    //sets variables for this button
    public void Config(int tipNumber)
    {
        currentTip = tipNumber;
    }
}
