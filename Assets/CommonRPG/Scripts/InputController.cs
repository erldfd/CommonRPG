using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{

    // assign the actions asset to this field in the inspector:
    [SerializeField]
    private InputActionAsset inputActionAsset;

    [SerializeField]
    private InputActionMap testInputActionMap;

    [SerializeField]
    private InputAction tesInputAction;

    [Serializable]
    private struct SInputActionInfo
    {
        [SerializeField]
        public string actionMapName;

        [SerializeField]
        public string actionName;
    }

    [SerializeField]
    private List<SInputActionInfo> inputActionInfoList = null;

    //[SerializeField]
    //// private field to store move action reference
    //private InputAction moveAction;
    //public InputAction MoveAction
    //{
    //    get { return moveAction; }
    //    set { moveAction = value; }
    //}

    void Awake()
    {
        // find the "move" action, and keep the reference to it, for use in Update
        //moveAction = actions.FindActionMap("PlayerInput").FindAction("Move");
   
        //moveAction.performed += OnMove;
        // for the "jump" action, we add a callback method for when it is performed

        inputActionAsset.FindActionMap("PlayerInput").FindAction("PauseAndResume").performed += OnPauseAndResume;
        //testInputActionMap.AddAction("Test").
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
        foreach (SInputActionInfo info in inputActionInfoList) 
        {
            inputActionAsset.FindActionMap(info.actionMapName).Enable();
        }

        foreach (SInputActionInfo info in inputActionInfoList)
        {
           // inputActionAsset.FindActionMap(info.actionMapName).FindAction(info.actionName).performed+=
        }

        inputActionAsset.FindActionMap("PlayerInput").Enable();
    }
    void OnDisable()
    {
        inputActionAsset.FindActionMap("PlayerInput").Disable();
    }
}
