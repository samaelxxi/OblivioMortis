using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;

namespace NodeCanvas.Tasks.Actions
{
    [Category("Oblivio/Movement")]
    [Description("Moves the agent towards to target per frame without pathfinding")]
    public class BruteJump : ActionTask<UnityEngine.AI.NavMeshAgent>
    {
        [RequiredField]
        public BBParameter<Vector3> Target;
        public BBParameter<float> Time = 1;
        public BBParameter<float> Height = 1;
        public BBParameter<AnimationCurve> Curve = new AnimationCurve();


        protected override void OnExecute()
        {
            agent.enabled = false;
            agent.transform.DOJump(Target.value, Height.value, 1, Time.value)
                                    .SetEase(Curve.value).WaitForCompletion();
            base.OnExecute();
        }

        protected override void OnUpdate()
        {
            if (elapsedTime >= Time.value)
            {
                EndAction();
                return;
            }
        }

        protected override void OnStop()
        {
            agent.enabled = true;
            base.OnStop();
        }
    }
}
