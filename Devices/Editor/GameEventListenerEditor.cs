using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GameEventListener))]
public class GameEventListenerEditor : Editor
{
    SerializedProperty _event;
    SerializedProperty _events;
    SerializedProperty _isComplex;
    SerializedProperty _condition;
    SerializedProperty _response;
    SerializedProperty _hasDelay;
    SerializedProperty _delayTime;
    SerializedProperty _onlyOnce;


    void OnEnable()
    {
        _event = serializedObject.FindProperty("Event");
        _events = serializedObject.FindProperty("Events");
        _isComplex = serializedObject.FindProperty("_isComplexCondition");
        _condition = serializedObject.FindProperty("Condition");
        _response = serializedObject.FindProperty("Response");
        _hasDelay = serializedObject.FindProperty("_hasDelay");
        _delayTime = serializedObject.FindProperty("_delayTime");
        _onlyOnce = serializedObject.FindProperty("_onlyOnce");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_isComplex);
        if (_isComplex.boolValue)
        {
            EditorGUILayout.PropertyField(_condition);
            EditorGUILayout.PropertyField(_events, true);
        }
        else
            EditorGUILayout.PropertyField(_event);

        // EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_hasDelay);
        if (_hasDelay.boolValue)
            EditorGUILayout.PropertyField(_delayTime);

        // EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_onlyOnce);

        // EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_response);

        serializedObject.ApplyModifiedProperties();
    }
}