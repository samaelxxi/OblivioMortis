using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DialogNPCData", menuName = "OblivioMortis/DialogNPCData", order = 0)]
public class DialogNPCData : ScriptableObject
{
    [field: SerializeField] 
    public string Name { get; private set; } = "Olle";
    [field: SerializeField] 
    public Sprite Avatar { get; private set; }
    // [SerializeField]
}
