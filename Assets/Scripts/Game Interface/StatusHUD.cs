using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusHUD : MonoBehaviour
{
    private Dictionary<string, TMP_Text> textDisplays = new Dictionary<string, TMP_Text>();
    public TMP_Text SumTopDisplay;
    public TMP_Text movesDisplay;
    private GameManager gm;

    private void Start()
    {
        
        TMP_Text[] displays = GetComponentsInChildren<TMP_Text>();
        foreach(TMP_Text display in displays)
        {
            textDisplays.Add(display.name.Trim(), display);
            //Debug.Log("myTest:"+display.name);
            
        }
        gm = GameManager.instance;
        
    }

    public void UpdateStatus()
    {

        textDisplays["SumTopDisplay"].text = gm.sumTop.ToString();
        textDisplays["MovesDisplay"].text = gm.playerStatus.movesTaken.ToString();
        
    }

    public void UpdateRessources()
    {
        textDisplays["EnergyDisplay"].text = "Energy:\n"+ gm.playerStatus.currentEnergy.ToString();
    }

}
