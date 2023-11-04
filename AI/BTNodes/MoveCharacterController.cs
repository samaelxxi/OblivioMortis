using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("Movement/Direct")]
    [Description("Moves the agent towards to target per frame without pathfinding")]
    public class MoveCharacterController : ActionTask<UnityEngine.AI.NavMeshAgent>
    {
        [RequiredField]
        public BBParameter<Vector3> target;
        public BBParameter<float> Time = 1;
        public BBParameter<AnimationCurve> Curve = new AnimationCurve();

        CharacterController _controller;


        Vector3 _startPos;

        protected override void OnExecute()
        {
            agent.enabled = false;
            _startPos = agent.transform.position;
            base.OnExecute();
            _controller = agent.GetComponent<CharacterController>();
        }

        protected override void OnUpdate()
        {
            if (elapsedTime >= Time.value)
            {
                EndAction();
                return;
            }

            var lerpValue = Curve.value.Evaluate(elapsedTime / Time.value);

            var targetPos = Vector3.Lerp(_startPos, target.value, lerpValue);
            var movePos = targetPos - agent.transform.position;
            _controller.Move(movePos);
        }

        protected override void OnStop()
        {
            agent.enabled = true;
            base.OnStop();
        }
    }

    [Category("Movement/Direct")]
    [Description("Moves the agent towards to target per frame without pathfinding")]
    public class MoveNavMesh : ActionTask<UnityEngine.AI.NavMeshAgent>
    {
        [RequiredField]
        public BBParameter<Vector3> target;
        public BBParameter<float> Time = 1;
        public BBParameter<AnimationCurve> Curve = new AnimationCurve();


        Vector3 _startPos;

        protected override void OnExecute()
        {
            // agent.enabled = false;
            _startPos = agent.transform.position;
            base.OnExecute();
            // _controller = agent.GetComponent<CharacterController>();
        }

        protected override void OnUpdate()
        {
            if (elapsedTime >= Time.value)
            {
                EndAction();
                return;
            }

            var lerpValue = Curve.value.Evaluate(elapsedTime / Time.value);
            var targetPos = Vector3.Lerp(_startPos, target.value, lerpValue);
            var movePos = targetPos - agent.transform.position;
            agent.Move(movePos);
        }

        protected override void OnStop()
        {
            // agent.enabled = true;
            base.OnStop();
        }
    }

    [Category("Movement/Direct")]
    [Description("Moves the agent forward per frame without pathfinding")]
    public class MoveNavMeshForward : ActionTask<UnityEngine.AI.NavMeshAgent>
    {
        [RequiredField]
        public BBParameter<float> Distance = 1;
        public BBParameter<float> Time = 1;
        public BBParameter<AnimationCurve> Curve = new AnimationCurve();


        float _prevDistance;

        protected override void OnExecute()
        {
            base.OnExecute();
            _prevDistance = 0;
        }

        protected override void OnUpdate()
        {
            if (elapsedTime >= Time.value)
            {
                EndAction();
                return;
            }

            var lerpValue = Curve.value.Evaluate(elapsedTime / Time.value);
            var newDistance = Mathf.Lerp(0, Distance.value, lerpValue);
            var ds = newDistance - _prevDistance;
            _prevDistance = newDistance;

            var movePos = agent.transform.position + agent.transform.forward * ds;
            var offset = movePos - agent.transform.position;
            agent.Move(offset);
        }
    }
}
