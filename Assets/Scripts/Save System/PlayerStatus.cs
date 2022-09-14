using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public GameManager gm;
    public string[] dicePowers;
    public string boardType, boardSkin, diceSkin, currentMatchCode,currentStateCode;
    public int movesTaken, undoCost, goalDone, scoreMatch, currentEnergy, spentEnergy;
    public Dictionary<string,int> matchsPlayed = new Dictionary<string, int>();
    public bool tutorial = true;




    public void EffectOfMovement()
    {
        movesTaken++;
    }

    //calculates final score in the end of a match
    public void Scoring()
    {
        if (movesTaken > 20)
        {
            scoreMatch = 10;
        }
        else
        {
            scoreMatch = 30 - movesTaken;
        }
        
    }

    //tries to spend the amount of energy passed, and returns true if scuccesful
    public bool SpendEnergy(int amount)
    {
        if (amount > currentEnergy)
        {
            return false;
        }
        else
        {
            currentEnergy -= amount;
            spentEnergy += amount;
            gm.statusHUD.UpdateRessources();
            return true;
        }
    }

    //updates the cost of undoing a move
    public void UpdateUndoCost(bool reset = false)
    {
        if (reset)
        {
            undoCost = 0;
        }
        else
        {
            if (undoCost == 0)
            {
                undoCost = 3;
            }
            else
            {
                undoCost *= 2;
                undoCost--;
            }
        }        
    }

    

    public PlayerReference SaveReference()
    {
        PlayerReference output = new PlayerReference();
        output.currentStateCode = gm.match.ExportCode(int.Parse(gm.match.startCode));
        output.goalDone = gm.goalTotal;
        output.movesTaken = movesTaken;
        output.currentEnergy = currentEnergy;
        output.spentEnergy = spentEnergy;
        output.undoCost = undoCost;
        

        //convert the dictionary
        output.matchsPlayedKeys = new string[matchsPlayed.Count];
        output.matchsPlayedValues = new int[matchsPlayed.Count];
        int i = 0;
        foreach (KeyValuePair<string,int> kvp in matchsPlayed)
        {
            output.matchsPlayedKeys[i] = kvp.Key;
            output.matchsPlayedValues[i] = kvp.Value;
            i++;
        }


        

        return output;
    }
    
    public void LoadReference(PlayerReference status)
    {
        currentStateCode = status.currentStateCode;
        goalDone = status.goalDone;
        movesTaken = status.movesTaken;
        currentEnergy = status.currentEnergy;
        spentEnergy = status.spentEnergy;
        undoCost = status.undoCost;
        //convert the dictionary
        matchsPlayed.Clear();
        for(int i = 0; i < status.matchsPlayedKeys.Length; i++)
        {
            string key = status.matchsPlayedKeys[i];
            int value = status.matchsPlayedValues[i];
            matchsPlayed.Add(key, value);
        }
        

    }
}
