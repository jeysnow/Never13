using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowButton : ButtonAction
{
    public string direction;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        direction = this.gameObject.name;
    }

    public override void Action()
    {
        gm.Move(direction);
    }

}
