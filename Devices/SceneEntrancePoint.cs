using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SelectionBase]
public class SceneEntrancePoint : MonoBehaviour
{
    [SerializeField] int _entranceIndex;

    public int EntranceIndex => _entranceIndex;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position.SetY(transform.position.y - 0.5f), Vector3.one * 1);
        Gizmos.DrawWireSphere(transform.position.SetY(transform.position.y + 0.5f), 0.5f);
        Gizmos.DrawLine(transform.position.SetY(transform.position.y + 0.5f), 
            transform.position.SetY(transform.position.y + 0.5f) + transform.forward);
    }
}
