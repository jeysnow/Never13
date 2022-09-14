using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseScreen : Screen
{
    

    protected override void Start()
    {
        base.Start();
        ConfigButton(myButtons["Undo"], "ButtonUndoAction");
        ConfigButton(myButtons["NewGame"], "ButtonNewGameAction");
        ConfigButton(myButtons["Replay"], "ButtonReplayAction");
    }


    public override void ShowScreen(bool show = true, Screen targetScreen = null)
    {
        base.ShowScreen(show, targetScreen);
        if (show && gm.actuallyPlaying)
        {
            gm.playerStatus.UpdateUndoCost();
            myButtons["Undo"].text.text = "Undo (" + gm.playerStatus.undoCost + ")";
        }
        
    }



    #region myButtons

    public void ButtonUndoAction()
    {
        //if you can pay, you'll go back to your game
        if (gm.playerStatus.SpendEnergy(gm.playerStatus.undoCost))
        {
            ShowScreen(false);
        }
        
    }
    public void ButtonNewGameAction()
    {
        gm.NewGame();
        ShowScreen(false);
    }
    public void ButtonReplayAction()
    {
        gm.NewGame(true);
        ShowScreen(false);
    }
    #endregion
}
