 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region scripts reference:
    public static GameManager instance;

    protected SaveSystem saveSystem;
    protected Die _activeDie;
    protected UserInterface userInterface;

    public StatusHUD statusHUD;
    public Board board;
    public Controls controls;
    public Dictionary<string, Die> dice = new Dictionary<string, Die>();
    public Match match;
    public PlayerStatus playerStatus;
    #endregion

    #region GameInfo
    [SerializeField]
    public bool actuallyPlaying = false;

    public int sumTop = 0, goalTotal = 0;
    public float boardCellSize;
    public string lastDirection;
    public bool movementDone = true, fastMode = false;
    public string[] validDestination = new string[0];

    #endregion

    #region Initialization
    protected virtual void Awake()
    {
        //singleton pattern
        if(GameManager.instance !=this&& GameManager.instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        
        
    }
    protected virtual void Start()
    {
        GetScripts();
        StartCoroutine(AfterStart());
    }

    public virtual IEnumerator AfterStart()
    {
        
        yield return new WaitForFixedUpdate();

        //loads the match arrays from current match type
        match.UpdateTypeCode();
        //saveSystem.Load("MatchReference");


        if (match.CheckCode(playerStatus.currentStateCode))
        {
            SetGame(playerStatus.currentStateCode, playerStatus.goalDone, playerStatus.movesTaken, playerStatus.undoCost);
            Debug.Log("myTest:"+"setgame Called");
        }
        else
        {
            NewGame();
            Debug.LogWarning("no previous state found to set game");
        }               
        
    }
    
    

    //goes to playscreen and sets puzzle
    public virtual void NewGame(bool retry = false)
    {
        string newCode, currentType = match.typeCode;
        Debug.Log("myTest:"+"Current type is: "+currentType);
        //make sure key is correct
        if (playerStatus.matchsPlayed.ContainsKey(currentType))
        {            
            //check if the player is repeating a level
            if (!retry)
            {
                
                //go to next match
                playerStatus.matchsPlayed[currentType]++;

                //check if you have not exceded amount of matches
                if (playerStatus.matchsPlayed[currentType] >= match.codeReference[currentType].Length)
                {
                    //let match 0 be the tutorial for that type
                    playerStatus.matchsPlayed[currentType] = 1;
                }
            }

            //this sets up the match selected above
            newCode = match.codeReference[currentType][playerStatus.matchsPlayed[currentType]];
            SetGame(newCode);
            
        }
        else
        {
            Debug.LogError("CurrentType not found in playerStatus dictionary");
        }

    }

    //sets puzzle
    public virtual void SetGame(string code, int goalDone = 0,int movesTaken = 0, int undoCost = 0)
    {
        if (match.CheckCode(code))
        {
            goalTotal = goalDone;
            playerStatus.movesTaken = movesTaken;
            playerStatus.undoCost = undoCost;
            match.ImportCode(code);
            controls.ResetControls();
            statusHUD.UpdateStatus();
            statusHUD.UpdateRessources();
        }
        else
        {
            Debug.LogError("game could not be set due to wrong code");
        }
        
    }



    //fills the dictionary dice with Die scripts from GameObjects tagged with "Die"
    public virtual void GetScripts()
    {
        GameObject[] gbs = GameObject.FindGameObjectsWithTag("Die");
        foreach (GameObject gb in gbs)
        {
            dice.Add(gb.name, gb.GetComponent<Die>());
        }
        board = FindObjectOfType<Board>();
        controls = FindObjectOfType<Controls>();
        statusHUD = FindObjectOfType<StatusHUD>();
        
        userInterface = GameObject.FindGameObjectWithTag("MainUI").GetComponent<UserInterface>();

        //relate to savesystem and its components
        saveSystem = SaveSystem.instance;
        match = SaveSystem.instance.match;
        playerStatus = SaveSystem.instance.playerStatus;
        match.gm = this;
        playerStatus.gm = this;
    }

    #endregion

    #region Movement
    public virtual void Move(string direction)
    {
        Debug.Log("myTest:"+movementDone);
        //make sure you don't activate this method twice before it's done.
        if (movementDone)
        {
            movementDone = false;
            lastDirection = direction;
            //Debug.Log("myTest:"+"1");
            MoveDie(direction);
            //Debug.Log("myTest:"+"2");
            MoveEffects(direction);
            //Debug.Log("myTest:"+"3");
            UpdateUI();
            //Debug.Log("myTest:"+"4");
            StartCoroutine(CheckEndGame());
            //Debug.Log("myTest:"+"5");
        }
    }

    public virtual IEnumerator CheckEndGame()
    {
        
        int timeout = 0;
        while (!movementDone &&timeout<1000)
        {
            yield return new WaitForEndOfFrame();
        }
        
        if (timeout >= 1000)
        {
            Debug.LogError("CheckEndGame timeout");
        }
        if (board.LoseCondition())
        {
            
            UndoLastMove();
            userInterface.StopMatch("Lost");
            
        }
        else if (board.WinCondition())
        {
            userInterface.StopMatch("Won");
        }
    }

    public void UpdateUI()
    {
        controls.UpdateArrows(activeDie);
        statusHUD.UpdateStatus();
        controls.RotateModel(true);

    }

    public virtual void MoveEffects(string direction)
    {

        board.Effects( CoordInateFromDirection(activeDie.coordinates,InvertDirection(direction)),activeDie.coordinates );
        activeDie.Effect(direction);
        playerStatus.EffectOfMovement();

    }

    //moves the dice on the board, uptades board and die.
    public void MoveDie(string direction)
    {
        //clears die current spot
        board.Clear(board.boardOccupants, activeDie.coordinates);

        //Move in direction to deired position and updates virtual die
        activeDie.Move(direction);

        //update coordenates and contents for new spot
        board.Put(board.boardOccupants, activeDie.coordinates, activeDie.nameId);
        //Debug.Log("myTest:"+"dice coordinates:" + d.x + "," + d.y);
                
    }

    //resets states after movement is done
    public void MoveEnd()
    {
        movementDone = true;
        controls.RotateModel(false);
    }
     
    //does the last movement in reverse
    public void UndoLastMove()
    {
        
        playerStatus.movesTaken -=2;
        Move(InvertDirection(lastDirection));        
    }

    //checks if the destination in a given direction is "Empty" and returns bool
    public bool CheckDestination(Die die, string direction)
    {
        //gets contents from board and occupants of the coordinate in the desired direction of current die.
        Coordinate destination = CoordInateFromDirection(die.coordinates, direction);
        string occupant = board.Locate(board.boardOccupants, destination);
        string spot = board.Locate(board.board, destination);

        //Debug.Log("myTest:"+"Checking " + direction + " from " + die + " and findig " + occupant + " as occupant and " + spot + " as spot.");

        //checks if both contents allow movement
        if (CheckValidDestination(occupant) && CheckValidDestination(spot))
        {
            
            return true;
        }
        return false;
    }

    //Checking and manipulating coordinates
    #region destination/direction
    public Coordinate CoordInateFromDirection(Coordinate currentCoordinates, string direction)
    {
        
        //new coordnate is necessary not to pass the path to the original object, but rather the internal values.
        Coordinate coord =  new Coordinate(currentCoordinates.coordinates);
        
        switch (direction)
        {
            case "Right":
                coord.x++;
                break;
            case "Left":
                coord.x--;
                break;
            case "Up":
                coord.y--;
                break;
            case "Down":
                coord.y++;
                break;
            default:
                Debug.LogError("wrong direction for DirectionFromCoordenate");
                break;
        }
        
        return coord;
    }

    //compares given string content toarray of valid strings
    public bool CheckValidDestination(string content)
    {        
        foreach (string s in validDestination)
        {
            if (s == content)
            {
                return true;                
            }
        }
        return false;
    }

    public string InvertDirection(string direction)
    {
        switch (direction)
        {
            case "Right":
                return "Left";
            case "Left":
                return "Right";
            case "Up":
                return "Down";
            case "Down":
                return "Up";
            default:
                Debug.LogError("wrong direction to inverse");
                return null;

        }
    }

    public bool CheckValueFrom(Die die,string direction)
    {
        if (die.faces[direction] - die.faces["Top"] + sumTop < 13)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #endregion



    protected virtual void OnApplicationQuit()
    {
        if (actuallyPlaying)
        {
            Debug.Log("myTest:"+"paused");
            saveSystem.Save("PlayerStatus");
        }
    }

    protected virtual void OnApplicationPause(bool pause)
    {
        if (actuallyPlaying)
        {
            Debug.Log("myTest:"+"paused");
            saveSystem.Save("PlayerStatus");
        }

    }


    #region special variables:

    public Die activeDie
    {
        get => _activeDie;
        set{
            try
            {
                _activeDie.Select(false);
            }
            catch
            {
                Debug.LogWarning("no previous active die");
            }
            _activeDie = value;
        }
    }


    #endregion


}
