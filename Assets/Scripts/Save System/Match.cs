using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : MonoBehaviour
{
    public GameManager gm;
    public string startCode, typeCode = "000-000-000-000-000";
    string[] powerList = new string[] {"SimpleDie" };
    string[] boardTypes = new string[] {"BasicBoard" };
    public Dictionary<string, string[]> codeReference = new Dictionary<string, string[]>();
    public MatchReference matchReference;


    public void UpdateTypeCode()
    {
        string fmt = "000";
        //make sure the typeofboard is on the list
        if (!boardTypes.Contains<string>(gm.board.typeOfBoard))
        {
            Debug.LogError("wrong boardtype");
            return;
        } 
        typeCode = Array.IndexOf(boardTypes, gm.board.typeOfBoard).ToString(fmt);

        //this guarantees the powers will be encoded in ascending order of powerlist array index
        #region convert powerlist
        
        List<int> powerListId = new List<int>();
        foreach (KeyValuePair<string,Die> kvp in gm.dice)
        {
            //make sure the powerID is on the list
            string power = kvp.Value.powerId;
            if (!powerList.Contains<string>(power))
            {
                typeCode = "";
                Debug.LogError("wrong dice power name from: " + kvp.Value.nameId);
                return;
            }

            //adds the power's position inside powerList to a listId to be sorted.
            powerListId.Add(Array.IndexOf(powerList, kvp.Value.powerId));
        }
        powerListId.Sort();

        //this writes the power into the typecode respecting format
        for(int i = 0; i < powerListId.Count; i++)
        {            
            typeCode += "-"+powerListId[i].ToString(fmt) ;
        }
        #endregion
    }

    //checks if matchcode is correctly formated
    public bool CheckCode(string code)
    {
        string[] components = code.Split(';');

        //makes sure the code format is correct
        if (components.Length != 6)
        {
            
            return false;
        }
        return true;
    }

    //currently imports serial number and four dice
    public void ImportCode(string code)
    {
        
        //Debug.Log("myTest:"+code);
        if (!CheckCode(code))
        {
            Debug.LogError("wrong code format imported");
            return;
        }
        string[] components = code.Split(';');
                
        
        //code's serial number
        startCode = components[0];

        //get the board ready
        gm.board.SetBoard();

        //make sure the sumtop down't accumulate between games;
        gm.sumTop = 0;

        //translate code to gm.dice, from third code entry to the end
        for (int i = 2; i < components.Length; i++)
        {
            //get data for each die
            string[] dieData = components[i].Split(',');
            //Debug.Log("myTest:"+"gm.dice[Diedata[0]] is: "+ dieData[0]);
            Die die = gm.dice[dieData[0].Trim()];
            
            //this updates target die Virtual and fisical rotation
            die.RotationSpecific(int.Parse(dieData[3]), int.Parse(dieData[4]));

            //updates virtual coordinates
            die.coordinates.coordinates = new int[] { int.Parse(dieData[1]), int.Parse(dieData[2]) };

            //updates button and model position
            die.MatchChildren();

            //updates board
            gm.board.Put(gm.board.boardOccupants, die.coordinates,die.nameId);

            gm.sumTop += die.faces["Top"];

        }
    }

    //code format: serialNumber;boardData(to be implemented);dieData;dieData;dieData;dieData/n
    public string ExportCode(int serialNumber)
    {
        //formats serial number to pattern
        string fmt = "000000";
        string code = serialNumber.ToString(fmt);

        //temporary content for board
        code += "; "+"rownumber" +","+ "collun number"+",";


        //gests dieData from all gm.dice
        foreach(KeyValuePair<string,Die> kvp in gm.dice)
        {
            Die die = kvp.Value;
            code += ";"+die.name + "," + die.coordinates.x + "," + die.coordinates.y + "," + die.faces["Top"] + "," + die.faces["Up"];
        }
        return code;
    }

    //called by savesystem to update references for the desired board and dice configuration
    public void LoadMatchference()
    {
        string[][] arrays = matchReference.CodeArrays;
        codeReference.Clear();
        for(int i = 0; i<arrays.Length;i++)
        {
            string key = arrays[i][0];
            string[] value = new string[arrays[i].Length-1];
            Array.Copy(arrays[i], 1,value,0,value.Length);
            //Debug.Log("myTest:"+"before add"+arrays[i][0]);
            codeReference.Add(key,value);
            //Debug.Log("myTest:"+"after add" + codeReference.ContainsKey(key));
        }
        //Debug.Log("myTest:"+"after for" + codeReference.ContainsKey(key);

    }

    //used in editor mode to save Matchreferences
    public void SaveMatchReference()
    {
        MatchReference output = new MatchReference();
        output.CodeArrays = new string[codeReference.Count][];
        int i = 0;
        foreach(KeyValuePair<string,string[]> kvp in codeReference)
        {
            string[] newAwrray = new string[kvp.Value.Length + 1];
            newAwrray[0] = kvp.Key;
            Array.Copy(kvp.Value, 0, newAwrray, 1, kvp.Value.Length);
            output.CodeArrays[i] = newAwrray;
            i++;
        }
        matchReference = output;
    }


    



}
