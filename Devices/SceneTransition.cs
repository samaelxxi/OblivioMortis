using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : InteractZone
{
    [SerializeField] string _sceneName;
    [SerializeField] int _entranceIndex;


    bool _isActivated = false;

    public override void OnInteraction()
    {
        if (_isActivated)
            return;

        ServiceLocator.Get<UIView>().EnableInteractBillboard(false);
        ServiceLocator.Get<SceneTransitionManager>().TransitionTo(_sceneName, _entranceIndex);
        _isActivated = true;
    }
}
