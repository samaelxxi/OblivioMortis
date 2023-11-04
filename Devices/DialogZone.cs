using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogZone : InteractZone, ISaveable
{
    [SerializeField] DialogData _dialogData;
    [SerializeField] bool _byTrigger = false;

    bool _isFinished = false;



    public override void OnTriggerEnter(Collider other)
    {
        if (!_byTrigger)
            base.OnTriggerEnter(other);
        else if (!_isFinished)
            OnInteraction();
    }

    public override void OnInteraction()
    {
        ServiceLocator.Get<UIView>().StartDialog(_dialogData, _isFinished);
        _isFinished = true;
    }

    public void Load(string data)
    {
        _isFinished = bool.Parse(data);
    }

    public string Save()
    {
        return _isFinished.ToString();
    }
}
