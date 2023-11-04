using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCastOverlap : MonoBehaviour
{
    [SerializeField] SphereCollider _sphereCollider;
    [SerializeField] BoxCollider _boxCollider;

    public void IsOverlap()
    {
        Collider[] _colliders = new Collider[5];
        var overlap = Physics.OverlapSphereNonAlloc(_sphereCollider.transform.position, 
                _sphereCollider.transform.lossyScale.x / 2, _colliders);
        Debug.Log($"Overlap: {overlap}");
    }

    public void MoveBackIfNeeded()
    {
        Collider[] _colliders = new Collider[5];
        var overlap = Physics.OverlapSphereNonAlloc(_sphereCollider.transform.position, 
                _sphereCollider.transform.lossyScale.x / 2, _colliders);
        
        while (Physics.OverlapSphereNonAlloc(_sphereCollider.transform.position, 
                _sphereCollider.transform.lossyScale.x / 2, _colliders) > 0)
        {
            Debug.Log($"Overlap: {overlap}");
            var direction = new Vector3(-1, 0, 0);
            _sphereCollider.transform.position += direction * 0.05f;
        }
    }

    public void SphereCast()
    {
        var hit = Physics.SphereCast(_sphereCollider.transform.position, 
                _sphereCollider.transform.lossyScale.x / 2, Vector3.right, out var hitInfo, 1);
        Debug.Log($"Hit: {hit}");
    }
}

