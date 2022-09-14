using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieView : MonoBehaviour
{
    public float rotateSpeed;
    public Quaternion rotationReference = Quaternion.identity;
    public MeshRenderer modelMeshRenderer;
    private Transform model;
    public bool rotateModel;
    public Material transparentMaterial;
    GameManager gm;

    private void Start()
    {
        gm = GameManager.instance;
        modelMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        model = transform.GetChild(0).GetComponent<Transform>();
        transparentMaterial = modelMeshRenderer.material;
        //Debug.Log("myTest:"+model.gameObject.name);
    }


    //checks if mouse clicks on this collider and drags (needs raycast)
    private void OnMouseDrag()
    {
        
        //converts the input mouse position
        float x, y;
        x = Input.GetAxis("Mouse X") * rotateSpeed;
        y = Input.GetAxis("Mouse Y") * rotateSpeed;

        //applies the input convertet to transform rotation
        transform.Rotate(Vector3.up, -x);
        transform.Rotate(Vector3.right, y);
    }

    //return to original position when released
    private void OnMouseUp()
    {        
        StartCoroutine(ReturnToRotation());
    }

    //slowly return to base rotation
    IEnumerator ReturnToRotation()
    {
        int timeout = 0;
        while(transform.rotation != Quaternion.identity&&timeout<100)
        {
            
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, rotateSpeed * Time.deltaTime);

            timeout++;

            yield return new WaitForEndOfFrame();
        }
        transform.rotation = Quaternion.identity;

    }


    public void UpdateModel(Material targetMaterial, Quaternion rotation)
    {
        modelMeshRenderer.material = targetMaterial;
        //rotationReference = rotation;
        model.rotation = rotation;
    }

    

    public IEnumerator Rotating()
    {
        int timeout = 0;
        while (rotateModel&& timeout <1000)
        {
            model.rotation = gm.activeDie.model.transform.rotation;
            timeout++;
            yield return new WaitForEndOfFrame();
        }
        if (timeout >= 1000)
        {
            Debug.LogError("Rotating dieview timeout");
        }
        
    }
}
