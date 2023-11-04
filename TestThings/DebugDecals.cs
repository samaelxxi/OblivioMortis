using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebugDecals", menuName = "ScriptableObjects/DebugDecals", order = 1)]
public class DebugDecals : Services.SORegistrable
{
    public void OnRegister(){}


    [field: SerializeField]
    public Material Default { get; private set; } = null;
    [field: SerializeField]
    public Material Attack { get; private set; } = null;
    [field: SerializeField]
    public Material Stagger { get; private set; } = null;
    [field: SerializeField]
    public Material Knockback { get; private set; } = null;
    [field: SerializeField]
    public Material AttackPrep { get; private set; } = null;
    [field: SerializeField]
    public Material AttackRest { get; private set; } = null;
    [field: SerializeField]
    public Material Damaged { get; private set; } = null;
}
