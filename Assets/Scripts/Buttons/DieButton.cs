using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DieButton : ButtonAction
{
    
    private RectTransform panel;
    private Die dieParent;
    private Controls controls;


    public float boardScreenWidht = 960, cellScreenWidht = 150, startScreenOffset = 100;
    public MeshRenderer meshParenDie;
    public DieView dieView;



    // creates a DieButton and sets basic references.
    public void Config(Die die)
    {
        Start();
        dieParent = die;        
        meshParenDie = die.model.GetComponent<MeshRenderer>();
        dieView = GameObject.FindGameObjectWithTag("ControlUI").GetComponentInChildren<DieView>();
        controls = GetComponentInParent<Controls>();
        controls.dieButtons.Add(dieParent.nameId, this);
    }
    
    public void Move()
    {
        rect.anchoredPosition = ToScreenPosition(dieParent.coordinates);
        //Debug.Log("myTest:"+"move called");
        
    }

    public Vector3 ToScreenPosition(Coordinate coord)
    {
        Coordinate crd = coord.Clone();
        //Debug.Log("myTest:"+"coordinates received: " + crd.x + ", " + crd.y);
        float x = (crd.x - 1) * cellScreenWidht + startScreenOffset;
        float y = ((crd.y - 1) * cellScreenWidht + startScreenOffset)*-1;
        return new Vector3(x,y);

    }

    public override void Action()
    {
        dieParent.Select(true);
        button.enabled = false;
        //Debug.Log("myTest:"+dieParent.name);

        //updates dieView with this parent's mesh and rotation
        dieView.UpdateModel(meshParenDie.material, dieParent.model.transform.rotation);
    }


}
