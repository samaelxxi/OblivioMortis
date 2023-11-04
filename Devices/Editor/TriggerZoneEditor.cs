using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TriggerZone))]
public class TriggerZoneEditor : Editor
{
    SerializedProperty _triggerType;
    SerializedProperty _onTriggerEvent;
    SerializedProperty _onTriggerCallback;

    void OnEnable()
    {
        _triggerType = serializedObject.FindProperty("_triggerType");
        _onTriggerEvent = serializedObject.FindProperty("_onTriggerEvent");
        _onTriggerCallback = serializedObject.FindProperty("_onTriggerCallback");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_triggerType);

        if (_triggerType.enumValueIndex == 0)
            EditorGUILayout.PropertyField(_onTriggerEvent);
        else
            EditorGUILayout.PropertyField(_onTriggerCallback);


        serializedObject.ApplyModifiedProperties();
    }
}
