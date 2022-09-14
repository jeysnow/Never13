using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    public Dictionary<string, Button> arrows = new Dictionary<string, Button>();
    public Dictionary<string, DieButton> dieButtons = new Dictionary<string, DieButton>();
    public DieView dieView;
    private GameManager gm;

    //checks if the destination for a particular die or active die (if null) 
    //and valid and disable respectivearrows if not
    public void UpdateArrows(Die die = null, bool checkValue = false)
    {
        if (die == null)
        {
            foreach (KeyValuePair<string, Button> kvp in arrows)
            {
                kvp.Value.enabled = false;
            }
        }
        else
        {
            foreach (KeyValuePair<string, Button> kvp in arrows)
            {
                if (gm.CheckDestination(die, kvp.Key) && (!checkValue || gm.CheckValueFrom(die, gm.InvertDirection(kvp.Key))))
                {

                    arrows[kvp.Key].enabled = true;
                }
                else
                {
                    arrows[kvp.Key].enabled = false;
                }
            }
        }
        
    }
        

    

    
    void Start()
    {
        gm = GameManager.instance;
        GetArrows();
        
        

    }

   

    public void ResetControls()
    {
        UpdateArrows();
        dieView.UpdateModel(dieView.transparentMaterial, Quaternion.identity);

        foreach(KeyValuePair<string,DieButton> kvp in dieButtons)
        {
            kvp.Value.button.enabled = true;
        }
    }

    public void RotateModel(bool rotate)
    {
        dieView.rotateModel = rotate;
        if (rotate)
        {
            StartCoroutine(dieView.Rotating());

        }
        

    }

    #region getReferences

    //gets button components of all 4 arrows.
    private void GetArrows()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Arrow");
        foreach (GameObject gb in buttons)
        {
            arrows.Add(gb.name, gb.GetComponent<Button>());

        }
        if (arrows.Count != 4)
        {
            Debug.LogError("wrong names for arrows");
        }

    }
    
    
    
    #endregion
}
