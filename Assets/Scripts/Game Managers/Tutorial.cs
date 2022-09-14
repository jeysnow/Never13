using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Tutorial : GameManager
{
    protected GameManager gm;
    public bool arrowPressed, sumTopOverboard, goalShown;
    public int tippingTime, timePast = 0;
    public RectTransform highLightScreen;
    public TMP_Text instructionText;
    public TutorialButton confirmButton;
    public string[] instructions;
    public Transform[] targetArray = new Transform[5];
    



    public override IEnumerator AfterStart()
    {
        yield return new WaitForFixedUpdate();

        //loads the match arrays from current match type
        match.UpdateTypeCode();
        //saveSystem.Load("MatchReference");

        NewGame(true);
        StartTutorial();

        StartCoroutine(WaitToTip());
    }


    public void StartTutorial()
    {

        foreach (KeyValuePair<string, Die> kvp in dice)
        {
            if (kvp.Value.nameId == "Yellow")
            {

            }
            else
            {
                kvp.Value.dieButton.gameObject.SetActive(false);
            }
        }

        

        Debug.Log("myTest:"+"Started");
    }

    
    public IEnumerator WaitToTip()
    {

        while (timePast<=tippingTime)
        {
            
            yield return new WaitForSecondsRealtime(0.1f);
               
            timePast++;

            if(timePast == tippingTime)
            {
                if (activeDie == null)
                {
                    //Debug.Log("myTest:"+"Point to Die");

                    ShowTip(0);

                    yield break;
                }
                else if (!goalShown)
                {
                    //Debug.Log("myTest:"+"Point to Goal");
                    goalShown = true;
                    ShowTip(1);
                    yield break;
                }
                else if (!arrowPressed)
                {
                    // Debug.Log("myTest:"+"Glow Arrow");
                    ShowTip(2);
                    yield break;
                }
                else
                {
                    yield break;
                }
                
            }
            
        }
        


    }

    public void ShowTip(int tipNumber)
    {
        //make sure the number passed was correct
        if (tipNumber >= instructions.Length || tipNumber < 0)
        {
            Debug.LogError("wrong tip numbber passed to ShowTip");
            return;
        }

        //sets the highlight Screen
        highLightScreen.gameObject.SetActive(true);
        highLightScreen.position = targetArray[tipNumber].position;

        //sets the text        
        instructionText.text = instructions[tipNumber];

        //sets the confirm button up
        confirmButton.ShowButton(true);
        confirmButton.Config(tipNumber);

        //position the text and set other configurations
        switch (tipNumber)
        {
            case 0:
                instructionText.transform.localPosition = new Vector3(3, -6);
                break;
            case 1:
                instructionText.transform.localPosition = new Vector3(1, -6);
                break;
            case 2:
                instructionText.alignment = TextAlignmentOptions.Right;
                instructionText.transform.localPosition = new Vector3(-11, 8);
                break;
            case 3:
                instructionText.alignment = TextAlignmentOptions.Right;
                instructionText.transform.localPosition = new Vector3(-13, -7);
                
                //stops waitingToTip while this instruction is beeing shown
                timePast = tippingTime + 1;
                
                break;
            case 4:
                Debug.Log("myTest:"+"Called");
                instructionText.transform.localPosition = new Vector3(9, 1);
                instructionText.alignment = TextAlignmentOptions.Left;
                timePast = tippingTime + 1;
                break;
            default:
                Debug.LogError("wrong tip numbber passed to ShowTip");
                return;

        }
        

        
    }



    public override void Move(string direction)
    {
        base.Move(direction);
        arrowPressed = true;
    }

    public override IEnumerator CheckEndGame()
    {
        
        if (sumTop > 12 && !sumTopOverboard)
        {
            ShowTip(3);
            sumTopOverboard = true;
            yield break;
        }
        
        StartCoroutine(base.CheckEndGame());
        
    }


    public override void GetScripts()
    {
        base.GetScripts();

        //gets targets for the positioning of screen tips
        GameObject[] targets = GameObject.FindGameObjectsWithTag("UItargets");
        foreach(GameObject obj in targets)
        {
            targetArray[int.Parse(obj.name.Trim())] = obj.transform;
        }
    }

    //checks if player tapped somewhere to reset wait time
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            timePast = 0;
        }
    }

    public override void NewGame(bool retry = false)
    {
        if (retry)
        {
            SetGame("000000; rownumber,collun number,;Red,4,3,3,2;Blue,4,4,3,6;Yellow,1,5,1,5;White,3,3,1,3");
        }
        else
        {
            playerStatus.matchsPlayed.Add(match.typeCode, 0);
            playerStatus.currentEnergy = playerStatus.scoreMatch;
            saveSystem.loadingScreen.ShowLoadScreen(2);
            //SceneManager.LoadScene();

        }
        
    }


}
