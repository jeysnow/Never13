using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ButtonAction : MonoBehaviour
{
    protected GameManager gm;
    public Button button;
    protected RectTransform rect;
    protected Image image;

    protected virtual void Start()
    {
        gm = GameManager.instance;
        button = GetComponent<Button>();
        rect = GetComponent<RectTransform>();
        button.onClick.AddListener(Action);
        image = GetComponent<Image>();
    }

    //to be overwriten with the actions of this button
    public abstract void Action();
    

    public virtual void ShowButton(bool show)
    {
        button.enabled = show;
        image.enabled = show;
    }
        
}
