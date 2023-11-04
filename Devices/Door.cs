using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

[SelectionBase]
public class Door : MonoBehaviour, ISaveable
{
    [SerializeField] bool _isClosedAtStart = true;
    [SerializeField] NavMeshObstacle _navMeshObstacle;
    Animator _animator;
    BoxCollider _doorCollider;


    bool _isLoaded = false;
    bool _isOpened = false;
    bool _changingState = false;


    void Awake()
    {
        _isOpened = !_isClosedAtStart;
        _animator = GetComponent<Animator>();
        _doorCollider = GetComponent<BoxCollider>();
    }

    void Start()
    {
        if (!_isLoaded && _isClosedAtStart)
            ChangeState(false, true);
    }

    public void OpenDoor()
    {
        if (!_isOpened && !_changingState)
            ChangeState(true);
    }

    public void CloseDoor()
    {
        if (_isOpened && !_changingState)
            ChangeState(false);
    }

    void ChangeState(bool isOpened, bool instantly = false)
    {
        _changingState = true;

        if (isOpened)
            _animator.Play("Open", layer: -1, normalizedTime: instantly ? 1 : 0);
        else
            _animator.Play("Close", layer: -1, normalizedTime: instantly ? 1 : 0);
        _isOpened = isOpened;
    }

    public void OnDoorClosed()
    {
        _changingState = false;
        _doorCollider.enabled = true;
        _navMeshObstacle.enabled = true;
    }

    public void OnDoorOpened()
    {
        _changingState = false;
        _doorCollider.enabled = false;
        _navMeshObstacle.enabled = false;
    }

    public string Save()
    {
        return _isOpened.ToString();
    }

    public void Load(string data)
    {
        _isOpened = bool.Parse(data);
        _isLoaded = true;
        if (_isOpened)
            ChangeState(true, true);
        else
            ChangeState(false, true);
    }
}
