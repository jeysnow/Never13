using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDice : MonoBehaviour
{
    public Transform flipper;

    //base value = 181
    [SerializeField]
    private float quertionSensitivity =181f;

    //you must create quartenions here for later use
    private Quaternion _targetRotation = new Quaternion();
    private Quaternion _currentRotation = new Quaternion();
    public float toppleSpeed = 1f, fixedHeight;

    private bool topple = false,adjust = false, fastMode;

    private string moveDir;
    private float cubeExtent;
    private Vector3 moveDestination;

    private void Start()
    {
        GetCubeExtent();
        //gets flipper ready to start
        flipper.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + cubeExtent);
        fastMode = GameManager.instance.fastMode;
        //Debug.Log("myTest:"+fastMode);
    }


    //define new flipper position based on directions: Up, Down, Left, Right
    private void SetTargetLocation(string direction){
        Vector3 newPosition = new Vector3();
        this.transform.SetParent(flipper.parent);      

        switch(direction){
            case "Right":
                newPosition = new Vector3(this.transform.position.x+cubeExtent,flipper.position.y,flipper.position.z);
                break;
            case "Left":
                newPosition = new Vector3(this.transform.position.x-cubeExtent,flipper.position.y,flipper.position.z);
                break;
            case "Down":
                newPosition = new Vector3(flipper.position.x, this.transform.position.y - cubeExtent, flipper.position.z);
                break;
            case "Up":
                newPosition = new Vector3(flipper.position.x, this.transform.position.y + cubeExtent, flipper.position.z);
                break;
            default:
                Debug.LogError("error on movement direction");
                break;
        }
        
        //sets flipper to desired position
        if(newPosition != null){
            flipper.position = newPosition;
            this.transform.SetParent(flipper);
        }

    }

    public void RotateToDirection(string direction)
    {
        Topple(direction,true);
    }

    public Quaternion SetTargetFlipperRotation(string dir){
        
        switch(dir){
            case "Right":
                return Quaternion.Euler(0, 90,0);
                
            case "Left":
                return Quaternion.Euler(0,-90,0);
                
            case "Down":
                return Quaternion.Euler(90,0,0);
                
            case "Up":
                return Quaternion.Euler(-90,0,0);
                
            default:
                Debug.LogError("wrong direction for model rotation");
                return Quaternion.identity;
        }
                
    }

    //checks if the rotation value in Euler is consistent with desired motion and corrects it when value reaches a treshold
    private void StabilizeRotation()
    {
        switch (moveDir)
        {
            //Each case verifies the rotation value on quartenion for the opposite value
            case "Right":

                if (_targetRotation.eulerAngles.y - flipper.rotation.eulerAngles.y > -quertionSensitivity)
                {
                    //Ensures rotation finishes correctly
                    flipper.rotation = Quaternion.Euler(0, -90, 0);
                    EndTopple();
                }
                break;

            case "Left":
                if (_targetRotation.eulerAngles.y - flipper.rotation.eulerAngles.y < quertionSensitivity)
                {
                    flipper.rotation = Quaternion.Euler(0, 90, 0);
                    EndTopple();
                }
                break;
            case "Down":

                if (_targetRotation.eulerAngles.x - flipper.rotation.eulerAngles.x > -quertionSensitivity)
                {
                    flipper.rotation = Quaternion.Euler(-90, 0, 0);
                    EndTopple();
                }
                break;

            case "Up":
                if (_targetRotation.eulerAngles.x - flipper.rotation.eulerAngles.x < quertionSensitivity)
                {
                    flipper.rotation = Quaternion.Euler(90, 0, 0);
                    EndTopple();
                }
                break;
            default:
                Debug.LogError("error on topple direction");
                break;
        }

    }

    //rotates the cube around the flipper intantly
    public void Topple(string direction, bool instant=false)
    {
        moveDir = direction;
        SetTargetLocation(direction);
        _targetRotation = SetTargetFlipperRotation(direction);
        topple = true;
        if (instant)
        {
            flipper.rotation = _targetRotation;
            StabilizeRotation();
            topple = false;
        }
        else
        {
            StartCoroutine(ToppleOverTime());
        }
        
    }
    
    //rotates the cube around the flipper with speed
    IEnumerator ToppleOverTime()
    {
        while (topple)
        {
            //excetutes rotations respectin speed per frame (-1 is to solve a minor fuck up 'o.o)
            flipper.rotation = Quaternion.LerpUnclamped(flipper.rotation, _targetRotation, Time.deltaTime * toppleSpeed * -1);

            //resets the  bool based on its direction
            StabilizeRotation();
            yield return new WaitForEndOfFrame();
        }
        
    }


    //adjust global variabels and objects when the topple ends
    private void EndTopple(){
        //stops update rotation
        topple = false;

        //unparents model from Flipper
        transform.SetParent(flipper.parent);

        //resets flipper's rotation
        flipper.rotation = Quaternion.identity;

        //makes sure the height is always correct
        transform.position = new Vector3(transform.position.x,transform.position.y, fixedHeight);

        //position the flipper in the middle of model's base.
        flipper.position = new Vector3(transform.position.x,transform.position.y,transform.position.z + cubeExtent);

    }

    //moves model during topple to assure it ends on a desired spot
    IEnumerator AdjustPositionToTarget()
    {
        while(adjust)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveDestination, Time.deltaTime * toppleSpeed / 10 );
            if(transform.position == moveDestination)
            {
                GameManager.instance.MoveEnd();
                adjust = false;
            }
            yield return new WaitForEndOfFrame();
        }
        
    }

    //starts the movement on update
    public void MoveDie(string dir,bool adjustMovement = false,Vector3 destination = new Vector3()){
        //Debug.Log("myTest:"+"Movedie called");
        //checks for conflicting instructions
        if(!topple){
            Topple(dir,fastMode);
        }
        if (adjustMovement&&!adjust)
        {
            
            if (fastMode)
            {
                transform.position = destination;
                GameManager.instance.movementDone = true;
            }
            else
            {
                moveDestination = destination;
                adjust = true;
                StartCoroutine(AdjustPositionToTarget());
            }
            
            
        }
        
    }

    //gets the size of the sube
    private void GetCubeExtent()
    {
        //checks if cube has all sizes the same.
        MeshFilter mf = this.GetComponent<MeshFilter>();
        if(mf.mesh.bounds.extents.x == mf.mesh.bounds.extents.y && mf.mesh.bounds.extents.y == mf.mesh.bounds.extents.z)
        {
            //adjust meshsize to scale of object
            cubeExtent = mf.mesh.bounds.extents.y * transform.localScale.y;
        }
        else
        {
            Debug.LogError("not a cube");
        }
    }

}
