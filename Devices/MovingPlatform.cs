using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KinematicCharacterController;

public class MovingPlatform : MonoBehaviour, IMoverController
{
    [SerializeField] float _speed = 1f;

    [SerializeField] protected List<Transform> _points = new();
    [SerializeField] protected Transform _platform;
    [SerializeField] protected Ease _ease = Ease.Linear;
    [SerializeField] protected PhysicsMover _mover;
    [SerializeField] protected bool _shouldLoop = true;
    // [SerializeField] Rigidbody _rigidbody;


    protected int _startPoint = 0;
    protected int _destinationPoint = 1;
    protected bool _forward = true;
    protected float _movingTime = 0;
    protected float _totalTime = 0;
    protected bool _isReady = false;

    [SerializeField] protected bool _isStopped = false;

    // Start is called before the first frame update
    protected Transform _transform;


    void Awake()
    {
        if (!enabled)
            _mover.enabled = false;
    }

    private void Start()
    {
        _transform = _platform;

        if (_points.Count < 2)
            Debug.LogError("Not enough points for moving platform");
        else if (!_points.TrueForAll(x => x != null))
            Debug.LogError("Some points of moving platform are null");
        else
            _isReady = true;

        _mover.MoverController = this;
        _movingTime = 0;
        SetNewDestination(0, 1);
    }

    public void SetEnabled(bool newEnabled)
    {
        _isStopped = !newEnabled;
    }

    // This is called every FixedUpdate by our PhysicsMover in order to tell it what pose it should go to
    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        if (!_isReady || _isStopped)
        {
            goalPosition = _platform.position;
            goalRotation = _platform.rotation;
            return;
        }

        _movingTime += deltaTime;

        // Remember pose before animation
        Vector3 _positionBeforeAnim = _transform.position;
        Quaternion _rotationBeforeAnim = _transform.rotation;

        // Set our platform's goal pose to the animation's
        float easeVal = GetCurrentTargetPos();
        goalPosition = _points[_startPoint].position;
        var dir = _points[_destinationPoint].position - _points[_startPoint].position;

        goalPosition += dir * easeVal;
        goalRotation = _transform.rotation;

        // Reset the actual transform pose to where it was before evaluating. 
        // This is so that the real movement can be handled by the physics mover; not the animation
        _transform.position = _positionBeforeAnim;
        _transform.rotation = _rotationBeforeAnim;

    }

    protected float GetCurrentTargetPos()
    {
        if (Vector3.Distance(_platform.position, _points[_destinationPoint].position) < 0.001f)
        {
            int nextPoint;
            if (_shouldLoop)
                nextPoint = (_destinationPoint + 1) % _points.Count;
            else
            {
                nextPoint = _destinationPoint + (_forward ? 1 : -1);
                if (nextPoint >= _points.Count || nextPoint < 0)
                {
                    _forward = !_forward;
                    nextPoint = _destinationPoint + (_forward ? 1 : -1);
                }
            }
            SetNewDestination(_destinationPoint, nextPoint);
        }
        var ease = DOVirtual.EasedValue(0, 1, _movingTime / _totalTime, _ease);
        return ease;
    }

    protected void SetNewDestination(int start, int destination)
    {

        _startPoint = start;
        _destinationPoint = destination;
        float totalDist = Vector3.Distance(_platform.position, _points[_destinationPoint].position);
        _movingTime = 0;
        _totalTime = totalDist / _speed;
    }

    protected void OnDrawGizmosSelected()
    {
        if (_points.Count < 2 || !_points.TrueForAll(p => p != null))
            return;

        for (int i = 0; i < _points.Count - 1; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_points[i].position, _points[i + 1].position);
        }
    }
}
