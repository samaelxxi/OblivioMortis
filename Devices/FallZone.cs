using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[RequireComponent(typeof(BoxCollider)), SelectionBase]
public class FallZone : MonoBehaviour
{
    [SerializeField] Transform _spawnPoint;
    

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.TeleportTo(_spawnPoint.position);
            player.TakeDamage(1, forced: true);
        }
    }


    #if UNITY_EDITOR
    BoxCollider _boxCollider;

    bool _showInEditor = false;
    public bool ShowInEditor => _showInEditor;


    void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    void OnDrawGizmos()
    {
        if (!_showInEditor)
            return;

        Gizmos.color = Color.red;
        Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(_boxCollider.transform.position, _boxCollider.transform.rotation, _boxCollider.transform.lossyScale);
        Gizmos.DrawWireCube(Vector3.zero, _boxCollider.size);
        Gizmos.matrix = oldGizmosMatrix;
        // // Gizmos.matrix = transform.localToWorldMatrix;
        // Gizmos.DrawWireCube(_boxCollider.bounds.center, _boxCollider.bounds.size);

        if (_spawnPoint)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_spawnPoint.position, 0.5f);
            Gizmos.DrawLine(transform.position, _spawnPoint.position);
        }
    }

    public void SetShowInEditor(bool value)
    {
        _showInEditor = value;
    }

    void OnValidate()
    {
        _showInEditor = ShowFallZones.ShowInEditor;
        if (_boxCollider == null)
            _boxCollider = GetComponent<BoxCollider>();
    }
    #endif
}
