
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif


public static class AnimatorExtensions
{
    #if UNITY_EDITOR
    public static void ChangeStateDuration(this Animator animator, string stateName, float newDuration)
    {
        AnimatorState state = animator.FindState(stateName);
        var clip = state.motion as AnimationClip;
        var speed = clip.length / newDuration;
        state.speed = speed;
    }

    public static AnimatorState FindState(this Animator animator, string name)
    {
        var controller = (AnimatorController)animator.runtimeAnimatorController;
        foreach (var layer in controller.layers)
        {
            var stateMachine = layer.stateMachine;
            foreach (var state in stateMachine.states)
                if (state.state.name == name)
                    return state.state;
            foreach (var stateMachine2 in stateMachine.stateMachines)
                foreach (var state2 in stateMachine2.stateMachine.states)
                    if (state2.state.name == name)
                        return state2.state;
        }
        return default;
    }
    #endif
}
