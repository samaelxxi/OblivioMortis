using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCaster : MonoBehaviour
{
    [SerializeField] float _sphereSizeRadius;
    [SerializeField] LineRenderer _lineRenderer;


    void Awake()
    {
        ServiceLocator.RegisterSO<BulletFactory>("Services/BulletFactory");
    }

    void OnDrawGizmos()
    {
        var currentPoint = transform.position;
        var rayDirection = transform.forward;
        var remainingLength = 100f;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(currentPoint, _sphereSizeRadius);

        while (true)
        {
            var hit = Physics.SphereCast(currentPoint, _sphereSizeRadius, rayDirection, out var hitInfo, remainingLength, PhysicsMasks.PlayerBulletTargetMask);

            if (hit)
            {
                //var projectedPoint = Vector3.Project((hitInfo.point - currentPoint),rayDirection)+currentPoint;
                // Debug.Log($"Hit {currentPoint} {rayDirection} {remainingLength} {hitInfo.point} {projectedPoint}");
                //var k = (projectedPoint - hitInfo.point).magnitude;
                //var t = Mathf.Sqrt(_sphereSizeRadius * _sphereSizeRadius - k * k);
                Vector3 sphereCenter;// = projectedPoint - rayDirection * t;

                // IM STUPID ALL ABOVE IS EQUALS TO THIS
                sphereCenter = hitInfo.point + hitInfo.normal * _sphereSizeRadius;
                // Debug.Log($"{k} {t} {sphereCenter}");


                Gizmos.color = Color.green;
                Gizmos.DrawLine(currentPoint, sphereCenter);
                Gizmos.DrawLine(hitInfo.point, sphereCenter);



                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(sphereCenter, _sphereSizeRadius);
                Gizmos.color = Color.red;


                remainingLength -= hitInfo.distance;
                currentPoint = sphereCenter;
                rayDirection = Vector3.Reflect(rayDirection, hitInfo.normal);
            }
            else
            {
                // Debug.Log($"No hit {currentPoint} {rayDirection} {remainingLength}");
                Gizmos.color = Color.green;
                Gizmos.DrawLine(currentPoint, currentPoint + rayDirection * remainingLength);
                break;
            }
        }
    }
}
