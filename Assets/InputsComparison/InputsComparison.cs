using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/**
 *  A simple comparison of Unity input types
 *  1. "OnMouse"     (old)   e.g. OnMouseDown() listeners
 *  2. "InputLegacy" (older) e.g. Input.GetMouseButtonDown(0) 
 *  3. "InputSystem" (new)   e.g. Mouse.current.leftButton.wasPressed
 *  
 *  Also, shows an example of the "Command" pattern, as each listener is routed to a "handler" function for outcomes
 *  https://gameprogrammingpatterns.com/command.html
 */

public class InputsComparison : MonoBehaviour
{
    public TMP_Text textObj;
    public TMP_Text textObjGeneral;
    public SpriteRenderer spriteRenderer;
    public Color32 color;

    public enum Mode { OnMouse, InputLegacy, InputSystem };
    public Mode mode;

    [Tooltip("To identify only the layers that can receive interaction")]
    public LayerMask interactableLayer;
    [Tooltip("User hovered/touched over object")]
    public bool hover;
    [Tooltip("User clicked/touched (not necessarily over object)")]
    public bool click;
    [Tooltip("User clicked/touched the object")]
    public bool hit;
    [Tooltip("Object that the user clicked/touched")]
    public GameObject hitObject;
    [Tooltip("Position of click/touch")]
    public Vector2 position;
    [Tooltip("Number of touches")]
    public int touchCount;
    [Tooltip("Current event category")]
    public string eventCategory;
    [Tooltip("Current event")]
    public string eventName;


    void OnValidate()
    {
        if (spriteRenderer == null) GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
    }
    void Update()
    {
        InputSystem_SavePointerPosition();
        InputLegacy_CheckMouseAndTouch();
        InputSystem_CheckPointer();
        Report();
    }






    /**
     *  OnMouseDown 
     *  https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnMouseDown.html
     *  - Requires a Collider on the object
     *  - Only works on an object itself
     *  - If there are several objects they ALL register the event (they are all always listening)
     *  - Works in Unity Remote
     */

    // Called when the mouse enters the Collider.
    void OnMouseEnter() =>
        OnMouse_MouseEventHandler(_hover: 1, _click: -1, _hitObject: gameObject, _eventName: "OnMouseEnter");
    // Called every frame while the mouse is over the Collider.
    void OnMouseOver() =>
        OnMouse_MouseEventHandler(_hover: 1, _click: -1, _hitObject: gameObject, _eventName: "OnMouseOver");
    // Called when the mouse is not any longer over the Collider.
    void OnMouseExit() =>
        OnMouse_MouseEventHandler(_hover: 0, _click: -1, _hitObject: null, _eventName: "OnMouseExit");
    // Called when the user has pressed the mouse button while over the Collider.
    void OnMouseDown() =>
        OnMouse_MouseEventHandler(_hover: 1, _click: 1, _hitObject: gameObject, _eventName: "OnMouseDown");
    // Called when the user has clicked on a Collider and is still holding down the mouse.
    void OnMouseDrag() =>
        OnMouse_MouseEventHandler(_hover: 1, _click: 1, _hitObject: gameObject, _eventName: "OnMouseDrag");
    // Called when the user has released the mouse button
    void OnMouseUp() =>
        OnMouse_MouseEventHandler(_hover: -1, _click: 0, _hitObject: null, _eventName: "OnMouseUp");
    // Called when the mouse is released over the same Collider as it was pressed.
    void OnMouseUpAsButton() =>
        OnMouse_MouseEventHandler(_hover: -1, _click: 0, _hitObject: null, _eventName: "OnMouseUpAsButton");

    /// <summary>
    /// Handle data from OnMouse events
    /// </summary>
    /// <param name="_hover">-1 means no change, 0=false, 1=true</param>
    /// <param name="_click">-1 means no change, 0=false, 1=true</param>
    /// <param name="_hitObject"></param>
    /// <param name="_eventName"></param>
    void OnMouse_MouseEventHandler(int _hover, int _click, GameObject _hitObject, string _eventName)
    {
        if (mode != Mode.OnMouse) return;
        UpdateEventCategory(gameObject, "OnMouse");
        // assign states
        if (_hover >= 0) hover = _hover == 1;
        if (_click >= 0) click = _click == 1;
        hitObject = _hitObject;
        SetSpriteState(hitObject);
        eventName = _eventName;
        Report();
    }











    /**
     *  Input.GetMouseButtonDown(0) 
     *  - Legacy Input https://docs.unity3d.com/ScriptReference/UnityEngine.InputLegacyModule.html
     *  - Returns true during the frame the user pressed the given mouse button.
     *  https://docs.unity3d.com/ScriptReference/Input.html
     *  e.g. https://docs.unity3d.com/ScriptReference/Input.GetMouseButtonDown.html
     *  - These detect any mouse event, regardless if you click while hovering over something. So you must use more code to detect collision
     *  - Works in Unity Remote
     */
    void InputLegacy_CheckMouseAndTouch()
    {
        if (mode != Mode.InputLegacy) return;

        // detect mouse hover => handle
        hitObject = CheckForHitObject(Input.mousePosition);
        if (Input.touchCount > 0 && hitObject == null) CheckForHitObject(Input.GetTouch(0).position);
        UpdateEventCategory(hitObject, "InputLegacy");
        InputLegacy_OnMouseHandler(1, -1, hitObject, "Hover", 0);
        InputLegacy_OnMouseHandler(1, -1, hitObject, "Hover", 1);
        InputLegacy_OnMouseHandler(1, -1, hitObject, "Hover", 2);


        // detect mouse clicks => handle
        if (Input.GetMouseButtonDown(0)) InputLegacy_OnMouseHandler(1, 1, hitObject, "Down", 0);
        if (Input.GetMouseButtonDown(1)) InputLegacy_OnMouseHandler(1, 1, hitObject, "Down", 1);
        if (Input.GetMouseButtonDown(2)) InputLegacy_OnMouseHandler(1, 1, hitObject, "Down", 2);
        // Returns true during the frame the user releases the given mouse button.
        if (Input.GetMouseButtonUp(0)) InputLegacy_OnMouseHandler(0, 0, hitObject, "Up", 0);
        if (Input.GetMouseButtonUp(1)) InputLegacy_OnMouseHandler(0, 0, hitObject, "Up", 1);
        if (Input.GetMouseButtonUp(2)) InputLegacy_OnMouseHandler(0, 0, hitObject, "Up", 2);
        // Returns whether the given mouse button is held down.
        if (Input.GetMouseButton(0)) InputLegacy_OnMouseHandler(1, 1, hitObject, "Held", 0);
        if (Input.GetMouseButton(1)) InputLegacy_OnMouseHandler(1, 1, hitObject, "Held", 1);
        if (Input.GetMouseButton(2)) InputLegacy_OnMouseHandler(1, 1, hitObject, "Held", 2);

        // detecting touches is different
        touchCount = Input.touchCount;
        // if touches => handle
        if (touchCount < 1) InputLegacy_OnTouchHandler(-1, -1, hitObject);
        else if (touchCount == 1) InputLegacy_OnTouchHandler(1, 1, hitObject);
        else if (touchCount == 2) InputLegacy_OnTouchHandler(1, 1, hitObject);
        // ... and so on.
    }


    void InputLegacy_OnMouseHandler(int _hover, int _click, GameObject _hitObject, string _eventName, int _buttonNumber)
    {
        // assign states
        if (_hover >= 0) hover = _hover == 1;
        if (_click >= 0) click = _click == 1;
        hitObject = _hitObject;
        SetSpriteState(hitObject);
        if (hitObject == null) return;
        eventName = $"Input.GetMouseButton{_eventName}({_buttonNumber})";
    }

    void InputLegacy_OnTouchHandler(int _hover, int _click, GameObject _hitObject)
    {
        // assign states
        if (_hover >= 0) hover = _hover == 1;
        if (_click >= 0) click = _click == 1;
        hitObject = _hitObject;
        if (hitObject == null) return;
        SetSpriteState(hitObject);
        eventName = $"Input.touches[{touchCount}].phase";
        InputSystem_DetectTouchPhase();
    }













    void InputSystem_SavePointerPosition()
    {
        touchCount = Input.touchCount;
        if (touchCount > 0 && Pointer.current.position.ReadValue() != null)
        {
            eventName = "Pointer.current.position";
        }
        else if (Mouse.current.position.ReadValue() != null)
        {
            position = Mouse.current.position.ReadValue();
            eventName = "Mouse.current.position";
        }
    }
    void InputSystem_DetectTouchPhase()
    {
        if (touchCount < 1) return;
        Touch touch = Input.GetTouch(touchCount - 1);

        // detect phase
        switch (touch.phase)
        {
            // when a touch has first been detected
            case UnityEngine.TouchPhase.Began:
                //message = "Begun ";
                break;

            // touch is a moving touch
            case UnityEngine.TouchPhase.Moved:
                //message = "Moving ";
                break;

            // touch is stationary
            case UnityEngine.TouchPhase.Stationary:
                //message = "Stationary";
                break;

            // touch has ended
            case UnityEngine.TouchPhase.Ended:
                //message = "Ending ";
                break;

            // touch has ended
            case UnityEngine.TouchPhase.Canceled:
                //message = "Canceled ";
                break;
        }
    }

    /**
     *  InputSystem
     *  https://docs.unity3d.com/Packages/com.unity.inputsystem@1.6/api/UnityEngine.InputSystem.html
     *  https://docs.unity3d.com/Packages/com.unity.inputsystem@1.6/manual/Pointers.html
     *  https://docs.unity3d.com/Packages/com.unity.inputsystem@1.6/manual/Migration.html
     *  - Works in Unity Remote
     */
    void InputSystem_CheckPointer()
    {
        if (mode != Mode.InputSystem) return;


        // figure out if we are looking for mouse vs. pointer events
        if (Input.touchCount > 0)
        {
            hitObject = CheckForHitObject(Pointer.current.position.ReadValue());
            UpdateEventCategory(hitObject, "InputSystem");
        }
        else
        {
            hitObject = CheckForHitObject(Mouse.current.position.ReadValue());
            UpdateEventCategory(hitObject, "InputSystem");
        }

        // hover
        if (hitObject != null) hover = true;
        else hover = false;

        SetSpriteState(hitObject);



        // touch / click
        if (Pointer.current.press.isPressed)
        {
            click = true;
            if (hitObject != null) eventName = $"Pointer.current";
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            click = true;
            if (hitObject != null) eventName = $"Mouse.current";
        }
        else click = false;


        InputSystem_DetectTouchPhase();
    }










    ////////////////////////////////////////////////////// 
    //////////////////// FUNCTIONS ///////////////////////
    //////////////////////////////////////////////////////

    void UpdateEventCategory(GameObject _hitObject, string _eventCategory)
    {
        if (_hitObject == null)
            eventCategory = "";
        else if (_hitObject == gameObject)
            eventCategory = _eventCategory;
    }
    public GameObject CheckForHitObject(Vector2 _position)
    {
        position = _position;
        // create a ray going from camera through a screen point
        Ray ray = Camera.main.ScreenPointToRay(position);
        // cast the 3D ray into the scene, returning ALL the Colliders along the ray
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, interactableLayer);
        // if hit then return the object, or default to null
        GameObject obj = null;
        if (hit && hit.collider)
            obj = hit.collider.gameObject;
        return obj;
    }
    void SetSpriteState(GameObject _hitObject)
    {
        hit = false;
        if (_hitObject == gameObject)
        {
            if (click)
            {
                hit = true;
                spriteRenderer.color = new Color32(color.r, color.g, color.b, 90);
            }
            else spriteRenderer.color = new Color32(color.r, color.g, color.b, 180);
        }
        else spriteRenderer.color = new Color32(color.r, color.g, color.b, 255);
    }
    void Report()
    {
        string txt = "";
        if (hitObject != null)
            txt = $"hitObject = {hitObject.name} \n";
        else
            txt = $"hitObject = null \n";
        txt +=
            $"hover = {hover} \n" +
            $"click = {click} \n" +
            $"hit = {hit} \n" +
            $"category = {eventCategory} \n" +
            $"event = {eventName} \n";
        textObj.text = txt;

        textObjGeneral.text =
            $"position = {position} \n" +
            $"touches = {touchCount} \n";

        //Debug.Log($"{gameObject.name}.{eventName}()");
    }


}





