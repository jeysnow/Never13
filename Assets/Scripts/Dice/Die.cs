using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Die : MonoBehaviour
{
    public string lastSpot, currentSpot, nameId, powerId;
    public Dictionary<string, int> faces = new Dictionary<string, int>();
    public Coordinate coordinates;
    public Button button;
    public MoveDice model;
    public DieButton dieButton;
    public GameObject dieButtonPrefab;



    GameManager gm;


    public virtual void Awake()
    {
        model = GetComponentInChildren<MoveDice>();
        nameId = gameObject.name;
        powerId = this.GetType().ToString();
        CreateButton();
    }

    private void Start()
    {
        gm = GameManager.instance;
        
        //Debug.Log("myTest:"+name+": my starting top face is: " + faces["Top"]);
        //Debug.Log("myTest:"+name+": my starting Up face is: " + faces["Up"]);
        //Debug.Log("myTest:"+name+": my starting rotation is: " + transform.rotation.eulerAngles);

    }

    private void CreateButton()
    {
        GameObject parent = GameObject.FindWithTag("BoardUI");
        GameObject temp = GameObject.Instantiate(dieButtonPrefab, parent.transform);
        dieButton = temp.GetComponent<DieButton>();
        dieButton.Config(this);
        dieButton.Move();
    }

    //rotates virtual and model die to specific Top/Up configuration
    public void RotationSpecific(int newTop,int newUp)
    {
        if(newTop == newUp || newTop + newUp == 7)
        {
            Debug.LogError("wrong faces inputed for rotation");
            return;
        }
        bool faceFound = false;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                RotateDice("Up");
                model.RotateToDirection("Up");
                //Debug.Log("myTest:"+"Upper Face of" + name + ":" + faces["Up"]);
                if (faces["Up"] == newUp)
                {
                    break;
                }
            }
            RotateDice("Right");
            model.RotateToDirection("Right");
            if (faces["Up"] == newUp)
            {
                faceFound = true;
                break;
            }
        }
        if (faceFound)
        {
            faceFound = false;
            for (int j = 0; j < 4; j++)
            {
                RotateDice("Right");
                model.RotateToDirection("Right");
                //Debug.Log("myTest:"+name + "went to rotation: " + model.transform.rotation);
                //Debug.Log("myTest:"+"Top Face of" + name + ":" + faces["Top"]);
                if (faces["Top"] == newTop)
                {
                    faceFound = true;
                    break;
                }
            }
            if (!faceFound)
            {
                Debug.LogError("Top face not found");
            }
        }
        else
        {
            Debug.LogError("Up face not found");
        }

        MatchChildren();
    }

    //sets random numbers for Up and Top, then rotates dice to match
    public void RotationRandom()
    {
        //this sequence guarantees two different random numbers
        int newTopNumber = Random.Range(1, 7);
        int newUpNumber = Random.Range(1, 7);
        while(newUpNumber == newTopNumber || newTopNumber+newUpNumber==7)
        {
            newUpNumber = Random.Range(1, 7);
        }

        RotationSpecific(newTopNumber, newUpNumber);
        
        

    }

    //Rotates the virtual die in a given direction (UP, Down, Left, Right)
    public void RotateDice(string direction)
    {
        
        switch (direction)
        {
            case "Up":
                faces["Up"] = (int)faces["Top"];
                faces["Top"] = (int)faces["Down"];
                CompleteDie();
                break;

            case "Down":
                faces["Top"] = (int)faces["Up"];
                faces["Up"] = (int)faces["Bottom"];
                CompleteDie();
                break;

            case "Left":
                faces["Top"] = (int)faces["Right"];
                faces["Right"] = (int)faces["Bottom"];
                
                CompleteDie();
                break;

            case "Right":
                faces["Right"] = (int)faces["Top"];
                faces["Top"] = (int)faces["Left"];
                CompleteDie();

                break;

            default:
                Debug.LogError("wrong direction to rotate");
                break;
        }
    }

    //Creates standard virtual die in standard rotation
    public Die()
    {
        BasicFaceState();
        coordinates = new Coordinate();
        
    }

    //start faces values
    public void BasicFaceState()
    {
        faces.Add("Top", 3);
        faces.Add("Bottom", 4);
        faces.Add("Up", 2);
        faces.Add("Down", 5);
        faces.Add("Right", 1);
        faces.Add("Left", 6);
    }

    //Completes die numbers from Top, Up and Right
    private void CompleteDie()
    {
        faces["Bottom"] = (int)(faces["Top"] - 7) * -1;
        faces["Down"] = (int)(faces["Up"] - 7) * -1;
        faces["Left"] = (int)(faces["Right"] - 7) * -1;
        //DebugFaces();        
         
    }

    //debugs the current faces of virtual die when called
    public void DebugFaces()
    {
        Debug.Log("myTest:"+"Die:" + name
         + " Top:" + faces["Top"]
         + " Up:" + faces["Up"]
         + " Right:" + faces["Right"]
         + " Bottom:" + faces["Bottom"]
         + " Down:" + faces["Down"]
         + " Left:" + faces["Left"]);

    }


    //define as activeDie on GameManager and turns glow on
    public virtual void Select(bool selected)
    {
        if (selected)
        {
            gm.activeDie = this;
            gm.controls.UpdateArrows(gm.activeDie);
        }
        else
        {
            dieButton.button.enabled = true;
        }
    }

    public void UpdateSpot(string newSpot)
    {
        lastSpot = currentSpot;
        currentSpot = newSpot;
    }

    //effects of die movement after movement (
    public virtual void Effect(string direction)
    {
        gm.sumTop += faces["Top"] - faces[direction];
        
    }
    
    //moves model, diebutton and updates virtual die
    public void Move(string direction)
    {
        //Debug.Log("myTest:"+"die move called");
        //updates virtual die
        coordinates = gm.CoordInateFromDirection(coordinates, direction);
        UpdateSpot(gm.board.Locate(gm.board.board,coordinates));
        RotateDice(direction);

        //moves model's transform
        Vector3 destination = gm.board.ToWorldPosition(coordinates);
        model.MoveDie(direction, true, new Vector3(destination.x, destination.y, model.fixedHeight));

        //moves DieButton in desired direction
        dieButton.Move();
        
    }

    //updates model and button position based on current coordinates
    public void MatchChildren()
    {
        dieButton.Move();
        model.transform.position = gm.board.ToWorldPosition(coordinates);
    }

}
