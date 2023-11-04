using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
{
    GameEvent Target { get { return (GameEvent)target; } }

    void DrawRaiseButton()
    {
        if (GUILayout.Button("Raise"))
            Target.Raise();
    }

    public override void OnInspectorGUI()
    {
        DrawRaiseButton();

        // draw listeners
        EditorGUILayout.LabelField("Listeners", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        for (int i = 0; i < Target.Listeners.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Target.Listeners[i].name);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                Target.UnregisterListener(Target.Listeners[i]);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}