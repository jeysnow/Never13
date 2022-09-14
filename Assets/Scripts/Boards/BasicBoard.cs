using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBoard : Board
{
    public int diceGoal = 4;
    protected override void OnEnter(string spot)
    {
        switch (spot)
        {
            case "Goal":
                GameManager.instance.goalTotal++;
                break;
            default:
                break;

        }

    }
    protected override void OnLeave(string spot)
    {
        switch (spot)
        {
            case "Goal":
                GameManager.instance.goalTotal--;
                break;
            default:
                break;

        }

    }

    public override bool WinCondition()
    {
        if(gm.goalTotal >= diceGoal)
        {
            return true;
        }
        return false;
    }

    public override void SetBoard()
    {
        base.SetBoard();
        ResizeBoard(6, 6);
        Coordinate goals = new Coordinate(Xcoord: 3, Ycoord: 3);
        Put(board, goals, "Goal");
        goals.x++;
        Put(board, goals, "Goal");
        goals.x--;
        goals.y++;
        Put(board, goals, "Goal");
        goals.x++;
        Put(board, goals, "Goal");
    }



    protected override void Start()
    {
        base.Start();
        SetBoard();

    }



}
