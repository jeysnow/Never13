using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board: MonoBehaviour
{
    #region Board Configs:
    protected int _rows, _colluns;
    protected GameManager gm;
    public int rows
    {
        get => _rows;
        
    }
    public int colluns
    {
        get => _colluns;
        
    }
    public float cellRealSize, startX, starY;
    public string typeOfBoard;

    public string[,] board = new string[6,6];
    public string[,] boardOccupants = new string[6, 6];
    protected Vector3[,] vector3s = new Vector3[6, 6];
    public string[] validOccupants = new string[0];
    public string[] validSpots = new string[0];
    #endregion
    //these are currently 0,792, -1.98 and 2.772
    
    
    //this function retrieves the name of the content of a particular spot on the array
    public string Locate(string[,] array, Coordinate coord)
    {
        Coordinate crd = new Coordinate(coord.coordinates);
        if (ValidateCoordinate(crd))
        {
            return array[crd.x - 1, crd.y - 1];
        }
        else
        {
            
            return "Wall";
        }

    }
    
    //this function overwrites a content on specified array and
    public virtual void Put(string[,] array, Coordinate coord, string content)
    {
        Coordinate crd = new Coordinate(coord.coordinates);
        if (ValidateCoordinate(crd))
        {
            if(ValidateContent(validOccupants,content) || ValidateContent(validSpots, content))
            {
                array[crd.x - 1, crd.y - 1] = content;

            }
            
        }        
    }

   


    //Overwrites every string in the array with "Empty", or a single coordenate if specified
    public void Clear(string[,] array,Coordinate coord = null)
    {
        if (coord == null)
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _colluns; j++)
                {
                    array[i, j] = "Empty";
                }
            }
        }
        else
        {
            Coordinate crd = coord.Clone();
            array[coord.x - 1, coord.y - 1] = "Empty";
        }
        
        
    }
        
    //tests if the coordenates are in the board
    public virtual bool ValidateCoordinate(Coordinate coord)
    {
        if (coord.x > 0 && coord.x < 7)
        {
            if (coord.y > 0 && coord.y < 7)
            {
                return true;
            }
            else
            {
                Debug.LogWarning("y out of range");
                return false;
            }
        }
        else
        {
            Debug.LogWarning("x out of range");
            return false;
        }

    }

    //tests if Occupant is on the array of valid names
    protected bool ValidateContent(string[] validContent, string content)
    {
        foreach (string s in validContent)
        {
            if (content == s)
            {
                return true;
                
            }
        }
        
        Debug.LogError("invalid content string");
        return false;
    }

    //sets the correct values for the cells based on the board.
    protected void DefineVectors()
    {
        
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _colluns; j++)
            {
                vector3s[i, j] = new Vector3(startX + i * cellRealSize, starY - j * cellRealSize);
            }
        }
        
        
    }

    //returns vector3 coordinate of board position (center of tile)
    public Vector3 ToWorldPosition(Coordinate coord)
    {
        if (ValidateCoordinate(coord))
        {
            
            return vector3s[coord.x - 1, coord.y - 1];
        }
        return new Vector3();
    }

    public virtual void Effects(Coordinate oldCoord, Coordinate newCoord)
    {
        OnLeave(Locate(board, oldCoord));
        OnEnter( Locate(board, newCoord));
        
    }

    protected virtual void OnLeave(string spot)
    {

    }
    protected virtual void OnEnter(string spot)
    {

    }

    //defines the condition to lose in each board. To be changed depending on board
    public virtual bool LoseCondition()
    {
        if (gm.sumTop >= 13)
        {
            return true;
        }
        return false;
    }

    //defines the condition to win in each board. To be changed depending on board
    public virtual bool WinCondition()
    {
        return false;
    }
    protected void ResizeBoard(int rowNumber, int colNumber)
    {
        _rows = rowNumber;
        _colluns = colNumber;
        board = new string[_rows, _colluns];
        boardOccupants = new string[_rows, _colluns];
        vector3s = new Vector3[_rows, _colluns];
        Clear(board);
        Clear(boardOccupants);
        DefineVectors();
    }

    public virtual void SetBoard()
    {

    }


    //this is called before start
    protected virtual void Awake()
    {
        typeOfBoard = this.GetType().ToString().Trim();


    }

    protected virtual void Start()
    {
        gm = GameManager.instance;
    }

}
