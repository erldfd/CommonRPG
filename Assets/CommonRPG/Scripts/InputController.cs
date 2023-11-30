using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{

    // assign the actions asset to this field in the inspector:
    [SerializeField]
    private InputActionAsset actions;

    [Serializable]
    private struct SInputContainor
    {
        [SerializeField]
        private InputActionAsset Testaction;

        [SerializeField]
        private Dictionary<string, string> TestMan;
    }

    [SerializeField]
    private Dictionary<string, string> TestMan;

    [SerializeField]
    private List<SInputContainor> inputContainors;

    //[SerializeField]
    //private string actionMapName = "";

    [SerializeField]
    // private field to store move action reference
    private InputAction moveAction;

    void Awake()
    {
        // find the "move" action, and keep the reference to it, for use in Update
        moveAction = actions.FindActionMap("PlayerInput").FindAction("Move");
   
        moveAction.performed += OnMove;
        // for the "jump" action, we add a callback method for when it is performed
        actions.FindActionMap("PlayerInput").FindAction("PauseAndResume").performed += OnPauseAndResume;
    }

    void Update()
    {
        // our update loop polls the "move" action value each frame
        //Vector2 moveVector = moveAction.ReadValue<Vector2>();
        //Debug.Log($"{moveVector}");
    }

    private void OnPauseAndResume(InputAction.CallbackContext context)
    {
        // this is the "jump" action callback method
        Debug.Log($"Jump! {context}");
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // this is the "jump" action callback method
        Debug.Log($"{context.ReadValue<Vector2>()}");
    }


    void OnEnable()
    {
        actions.FindActionMap("PlayerInput").Enable();
    }
    void OnDisable()
    {
        actions.FindActionMap("PlayerInput").Disable();
    }
}
