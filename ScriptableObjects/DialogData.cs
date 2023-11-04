using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DialogLines", menuName = "OblivioMortis/DialogData", order = 0)]
public class DialogData : ScriptableObject
{
    [SerializeField] DialogNPCData _npc;
    [SerializeField] List<string> _lines;
    [SerializeField] string _endLine;

    public DialogNPCData NPC => _npc;
    public List<string> Lines => _lines;
    public string EndLine => _endLine;
}
