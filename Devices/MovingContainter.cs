using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KinematicCharacterController;
using UnityEngine;

public class MovingContainter : MonoBehaviour, IMoverController
{
    [SerializeField] float _speed = 1f;
    [SerializeField] Transform _platform;
    [SerializeField] protected Ease _ease = Ease.Linear;
    [SerializeField] Transform _startPoint;
    [SerializeField] Transform _endPoint;
    [SerializeField] protected PhysicsMover _mover;
    [SerializeField] float _delay = 2;

    float _delayTime = 0;
    float _movingTime = 0;
    float _totalTime = 0;
    Vector3 _dir;
    float _distance;

    void Awake()
    {
        _delayTime = _delay;
        _mover.MoverController = this;
        OnValidate();
    }

    void OnValidate()
    {
        _dir = (_endPoint.position - _startPoint.position).normalized;
        _distance = Vector3.Distance(_startPoint.position, _endPoint.position);
        _totalTime = Vector3.Distance(_startPoint.position, _endPoint.position) / _speed;
    }

    // This is called every FixedUpdate by our PhysicsMover in order to tell it what pose it should go to
    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        if (_delayTime < _delay)
        {
            _delayTime += deltaTime;
            goalPosition = _startPoint.position;
            goalRotation = _platform.rotation;
            return;
        }

        _movingTime += deltaTime;

        float easeVal;
        easeVal = DOVirtual.EasedValue(0, 1, _movingTime / _totalTime, _ease);
        
        if (_movingTime > _totalTime)
        {
            _delayTime = 0;
            _movingTime = 0;
            // _goingForward = !_goingForward;
            TeleportPlatform();
        }
        // var dir = _goingForward ? _dir : -_dir;
        goalPosition = _startPoint.position + _distance * easeVal * _dir;
        goalRotation = _platform.rotation;
    }

    void TeleportPlatform()
    {
        _platform.GetComponent<MeshRenderer>().enabled = false;
        _platform.GetComponent<BoxCollider>().enabled = false;
        _mover.SetPosition(_startPoint.position);
        this.InSeconds(_delay, () =>
        {
            _platform.GetComponent<MeshRenderer>().enabled = true;
            _platform.GetComponent<BoxCollider>().enabled = true;
        });
    }
    
    void OnDrawGizmosSelected()
    {
        if (_startPoint != null && _endPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_startPoint.position, _endPoint.position);
        }
    }
}