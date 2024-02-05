using CommonRPG;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConversationDataScriptableObject))]
public class ConversationDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Open Conversation Maker Window"))
        {
            var window = ConversationMakerWindow.ShowWindow();
            window.SetScriptableObject((ConversationDataScriptableObject)target);
        }
    }
}
