using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class StageEditor : GameManager
{
    public InputField inputMatchSerial, diePositionX, diePositionY, dieFaceTop, dieFaceUp;
    public Dropdown inputMatchType;
    //public Button loadMatchType, loadMatchSerial, saveMatchSerial;


    //don't start anything untill a button is called
    public override IEnumerator AfterStart()
    {
        yield return new WaitForEndOfFrame();

        match.UpdateTypeCode();

        //loads current reference
        saveSystem.Load("MatchReference");

        //puts codeTypes in the dropdown menu
        inputMatchType.ClearOptions();
        inputMatchType.AddOptions(match.codeReference.Keys.ToList<string>());
        Debug.Log("myTest:"+inputMatchType.options[inputMatchType.value].text.Trim());

        
    }


    public void LoadMatch()
    {
        string key = inputMatchType.options[inputMatchType.value].text.Trim();
        int index = int.Parse(inputMatchSerial.text);
        match.ImportCode(match.codeReference[key][index]);
        controls.UpdateArrows();
        statusHUD.UpdateStatus();

    }

    public void SaveMatch()
    {
        string key = inputMatchType.options[inputMatchType.value].text.Trim();
        int index = int.Parse(inputMatchSerial.text);
        string codeToSave = match.ExportCode(index);
        match.codeReference[key][index] = codeToSave;
        saveSystem.Save("MatchReference");
    }

    public void SetDieRotation()
    {
        if(dieFaceTop.text !="" && dieFaceUp.text != "")
        {
            //this updates target die Virtual and fisical rotation
            activeDie.RotationSpecific(int.Parse(dieFaceTop.text), int.Parse(dieFaceUp.text));

        }
        else
        {
            Debug.LogError("not enough faces provided to set rotation");
        }

    }


    public void SetDiePosition()
    {
        Debug.Log("myTest:"+"called");
        
        //updates virtual coordinates
        activeDie.coordinates.coordinates = new int[] { int.Parse(diePositionX.text), int.Parse(diePositionY.text) };

        //updates button and model position
        activeDie.MatchChildren();

        //updates board
        board.Put(board.boardOccupants,activeDie.coordinates, activeDie.nameId);
    }

}
