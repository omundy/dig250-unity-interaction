using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Adds ability to select an object in the scene using new InputSystem
 */

public class ObjectSelect : MonoBehaviour
{
    Material material;

    public bool hover;
    public float colorInterval = 0.15f;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    private void Update()
    {
    }



    // OnMouseEnter occurs on the first frame the mouse is over the object
    // OnMouseOver is  called each frame until the mouse moves away, at which point OnMouseExit is called.
    private void OnMouseEnter()
    {
        UpdateAlpha(true);
    }
    private void OnMouseOver()
    {
        //UpdateAlpha(true);
    }
    private void OnMouseExit()
    {
        UpdateAlpha(false);
    }
    private void OnMouseDown()
    {
        CycleColor();
    }
    private void OnMouseUp()
    {
        CycleColor();
    }



    private void UpdateAlpha(bool state = true)
    {
        hover = state;
        // get current color
        Color c = material.GetColor("_Color");
        Debug.Log("UpdateAlpha() state =" + state);
        if (!hover)
            material.SetColor("_Color", new Color(c.r, c.g, c.b, 1));
        else
            material.SetColor("_Color", new Color(c.r, c.g, c.b, 0.7f));
    }

    private void CycleColor()
    {
        // get current color
        Color c = material.GetColor("_Color");
        Debug.Log("UpdateColor() " + c.ToString());

        // b
        if (c.b < 1)
        {
            c.b = NextColor(c.b);
        }
        // g
        else if (c.g < 1)
        {
            c.g = NextColor(c.g);
            c.b = 0;
        }
        // r
        else if (c.r < 1)
        {
            c.r = NextColor(c.r);
            c.g = 0;
            c.b = 0;
        }
        // reset
        else
        {
            c = new Color(0, 0, 0, c.a);
        }

        // set color
        material.SetColor("_Color", new Color(c.r, c.g, c.b, c.a));
    }


    float NextColor(float _c)
    {
        return Mathf.Min(1, _c + colorInterval);
    }


}
