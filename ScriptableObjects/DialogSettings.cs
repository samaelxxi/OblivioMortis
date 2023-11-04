using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


// [CreateAssetMenu(fileName = "DialogSettings", menuName = "ScriptableObjects/DialogSettings", order = 0)]
public class DialogSettings : ScriptableObject
{
    [field: SerializeField]
    public bool AutoGoToNextLine { get; private set; } = true;

    [field: SerializeField]
    public float DelayBeforeStart { get; private set; } = 0f;
    [field: SerializeField]
    public float LetterTime { get; private set; } = 0.04f;
    [field: SerializeField]
    public float SpaceTime { get; private set; } = 0.03f;
    [field: SerializeField]
    public float DotTime { get; private set; } = 0.3f;
    [field: SerializeField]
    public float CommaTime { get; private set; } = 0.15f;
    [field: SerializeField, ShowIf("AutoGoToNextLine")]
    public float EndTime { get; private set; } = 1;
}
