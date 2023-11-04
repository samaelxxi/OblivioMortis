using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Lever))]
public class LeverEditor : Editor
{
    SerializedProperty _twoWay;
    SerializedProperty _leverType;
    SerializedProperty _onEnableEvent;
    SerializedProperty _onDisableEvent;
    SerializedProperty _onEnableCallback;
    SerializedProperty _onDisableCallback;
    SerializedProperty _canBeAttacked;
    SerializedProperty _attackCollider;
    SerializedProperty _animator;


    void OnEnable()
    {
        _twoWay = serializedObject.FindProperty("_twoWay");
        _leverType = serializedObject.FindProperty("_leverType");
        _onEnableEvent = serializedObject.FindProperty("_onEnableEvent");
        _onDisableEvent = serializedObject.FindProperty("_onDisableEvent");
        _onEnableCallback = serializedObject.FindProperty("_onEnableCallback");
        _onDisableCallback = serializedObject.FindProperty("_onDisableCallback");
        _canBeAttacked = serializedObject.FindProperty("_canBeAttacked");
        _attackCollider = serializedObject.FindProperty("_attackCollider");
        _animator = serializedObject.FindProperty("_animator");
    }



    bool showPosition = false;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_twoWay);
        EditorGUILayout.PropertyField(_canBeAttacked);
        EditorGUILayout.PropertyField(_leverType);
        if (_leverType.enumValueIndex == 0)
        {
            EditorGUILayout.PropertyField(_onEnableEvent);
            if (_twoWay.boolValue)
                EditorGUILayout.PropertyField(_onDisableEvent);
        }
        else
        {
            EditorGUILayout.PropertyField(_onEnableCallback);
            if (_twoWay.boolValue)
                EditorGUILayout.PropertyField(_onDisableCallback);
        }

        showPosition = EditorGUILayout.Foldout(showPosition, "Internals");
        if (showPosition)
        {
            if (Selection.activeTransform)
            {
                EditorGUILayout.PropertyField(_attackCollider);
                EditorGUILayout.PropertyField(_animator);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
