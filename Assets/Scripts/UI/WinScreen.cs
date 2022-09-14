using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinScreen : Screen
{
    private TMP_Text scoreText;
    private int scoreNumber = 0;

    protected override void Start()
    {
        base.Start();
        scoreText = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        ConfigButton(myButtons["NewGame"], "ButtonNewGameAction");
        ConfigButton(myButtons["Replay"], "ButtonReplayAction");

    }

    public void ShowScore(int score)
    {
        StartCoroutine(ScoreTextUpdate(score));
    }

    private IEnumerator ScoreTextUpdate(int score)
    {
        while (scoreNumber <= score)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            scoreText.text = "Final Score: " + scoreNumber;
            scoreNumber++;

        }
        scoreNumber = 0;
        
    }

    #region myButtons

    public void ButtonNewGameAction()
    {
        gm.playerStatus.currentEnergy += gm.playerStatus.scoreMatch;
        gm.statusHUD.UpdateRessources();
        gm.NewGame();
        ShowScreen(false);
    }
    public void ButtonReplayAction()
    {
        gm.playerStatus.scoreMatch = 0;
        gm.NewGame(true);
        ShowScreen(false);
    }
    #endregion
}
