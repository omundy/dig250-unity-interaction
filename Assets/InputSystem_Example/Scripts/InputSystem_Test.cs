using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


/**
 *  Test script for InputSystem
 *  - Search for InputSystem in classes for other examples:
 *  References:
 *  https://www.youtube.com/watch?v=kGykP7VZCvg
 *  https://learn.unity.com/tutorial/taking-advantage-of-the-input-system-scripting-api?uv=2020.1
 *  https://stackoverflow.com/a/72941047/441878
 */

public class InputSystem_Test : MonoBehaviour, InputControls.ITestActions
{
    // reference to InputSystem controls
    private InputControls inputControls;


    // for movement example
    [SerializeField] Vector2 move;
    public float speed = 10;
    [SerializeField] Vector2 mousePosition;

    // to be able to list them all
    public InputActionAsset actionAsset;


    private void Awake()
    {
        // create instance of InputSystem controls
        inputControls = new InputControls();
    }

    private void OnEnable()
    {
        // subscribe to events

        // assign callback to performed ("mouse down"), using Lamda (context parameter is not passed)
        inputControls.Test.LeftButtonMouse.started += context => LeftButtonMouseDown();

        // assign callback to canceled ("mouse up"), passing context parameter
        inputControls.Test.LeftButtonMouse.canceled += LeftButtonMouseUp;


        // BAD ASSIGNMENT EXAMPLES
        // - you can't unsubscribe so there will be a memory leak!!

        // assign callback, reading/converting parameter to Vector2 for receiving function
        inputControls.Test.Move.performed += ctx => MoveExample(ctx.ReadValue<Vector2>());

        // performed/canceled callbacks, updating the "move" variable
        // - this will work with WASD or arrows (see controls)
        inputControls.Test.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        inputControls.Test.Move.canceled += ctx => move = Vector2.zero;


        // another way subscribe to InputActions
        inputControls.Test.Move.performed += HandleInputAction;
        inputControls.Test.Move.canceled += HandleInputAction;


        // enable the entire action map
        inputControls.Test.Enable();
    }
    private void OnDisable()
    {
        // unsubscribe (use same signatures!) to events
        inputControls.Test.LeftButtonMouse.started -= context => LeftButtonMouseDown();
        inputControls.Test.LeftButtonMouse.canceled -= LeftButtonMouseUp;
        inputControls.Test.Move.performed -= ctx => MoveExample(ctx.ReadValue<Vector2>());
        inputControls.Test.Move.performed -= ctx => move = ctx.ReadValue<Vector2>();
        inputControls.Test.Move.canceled -= ctx => move = Vector2.zero;


        // another way subscribe to InputActions
        inputControls.Test.Move.performed -= HandleInputAction;
        inputControls.Test.Move.canceled -= HandleInputAction;

        // disable the entire action map
        inputControls.Test.Disable();
    }


    /**
     *  CALLBACKS
     */

    void LeftButtonMouseDown()
    {
        Debug.Log("LeftButtonMouseDown()");
    }
    void LeftButtonMouseUp(InputAction.CallbackContext context)
    {
        // print value as float
        Debug.Log("LeftButtonMouseUp() " + context.ReadValue<float>());
    }
    void MoveExample(Vector2 coordinates)
    {
        // print value read from listener
        Debug.Log("Move() coordinates = " + coordinates);
    }
    void SendMessage(InputAction.CallbackContext context)
    {
        Debug.Log("inputControls.Test.HelloWorld.canceled => ");
    }


    /// <summary>
    /// Handle all UnityEvents from InputSystem
    /// </summary>
    /// <param name="context"></param>
    private void HandleInputAction(InputAction.CallbackContext context)
    {
        Debug.Log("context.action.name = " + context.action.name);

        // another way

        if (context.action.name == "DisplayNextSlide")
        {
            // logic
        }
        else if (context.action.name == "DisplayPreviousSlide")
        {
            // logic
        }
        else if (context.action.name == "RestartSlides")
        {
            // logic
        }

        // access to all phases
        if (context.started) Debug.Log("Event started");
        else if (context.performed) Debug.Log("Event is continuing");
        else if (context.canceled) Debug.Log("Event canceled");

        // yet another way to access phases
        switch (context.phase)
        {
            case InputActionPhase.Started:
            case InputActionPhase.Performed:
                // logic
                break;
            case InputActionPhase.Canceled:
                // logic
                break;
        }

    }




    /**
     *  EXAMPLES
     */

    // list all actions maps
    void ListMapsOnAsset(InputActionAsset asset)
    {
        foreach (InputActionMap map in asset.actionMaps)
        {
            Debug.Log("map.name = " + map.name);
        }
    }
    // find an action map
    InputActionMap ReturnActionMap(string str)
    {
        return inputControls.asset.FindActionMap(str);
    }
    // list all actions on a map
    void ListActionsOnMap(InputActionMap map)
    {
        //foreach (var action in actionAsset) // directly on the serialized field
        foreach (InputAction action in map) // getting the asset from the obj declared above
        {
            Debug.Log(map.name + " => " + action.name);
        }
    }


    private void Start()
    {
        // Show all devices in the system
        Debug.Log(string.Join("\n", InputSystem.devices));
        Debug.Log(string.Join("\n", Gamepad.all));


        // more details
        // https://adventurecreator.org/forum/discussion/10748/a-more-seamless-integration-with-the-new-unity-input-system-with-some-issues

        ListMapsOnAsset(inputControls.asset);
        InputActionMap map = ReturnActionMap("Test");
        ListActionsOnMap(map);
    }

    private void Update()
    {
        // you can also get device input directly without setting up any Actions or Control Schemes
        // https://gamedevbeginner.com/input-in-unity-made-easy-complete-guide-to-the-new-system/

        mousePosition = Mouse.current.position.ReadValue();

        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            // you can get actual keys with:
            // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.3/api/UnityEngine.InputSystem.Keyboard.html#UnityEngine_InputSystem_Keyboard_onTextInput
            Debug.Log("A key was pressed");
        }
        // has a check to make sure it exists
        if (Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame)
        {
            Debug.Log("A button was pressed");
        }
    }


    void FixedUpdate()
    {
        Vector3 movement = new Vector3(move.x, 0.0f, move.y) * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }





    public void OnLeftButtonMouse(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }
}
