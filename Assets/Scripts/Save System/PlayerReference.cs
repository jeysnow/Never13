using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerReference
{
    public string currentStateCode;
    public int goalDone, movesTaken, currentEnergy, spentEnergy, undoCost;
    public string[] matchsPlayedKeys;
    public int[] matchsPlayedValues;       
}
