using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{
    public enum Plane { XZ, XY, YZ }
    [Category("Movement/Direct")]
    [Description("Rotate the agent towards the target per frame ignoring one axis")]
    public class RotateTowardsOnPlane : ActionTask<Transform>
    {
        [RequiredField]
        public BBParameter<GameObject> target;
        public BBParameter<float> speed = 2;
        [SliderField(1, 180)]
        public BBParameter<float> angleDifference = 5;
        public BBParameter<Vector3> upVector = Vector3.up;
        public BBParameter<Plane> plane = Plane.XZ;
        public bool waitActionFinish;

        protected override void OnUpdate()
        {
            Vector3 targetPos = Vector3.zero;
            Vector3 agentPos = Vector3.zero;
            Vector3 agentForward = Vector3.zero;
            switch ( plane.value )
            {
                case Plane.XZ:
                    targetPos = target.value.transform.position.SetY(0);
                    agentPos = agent.position.SetY(0);
                    agentForward = agent.forward.SetY(0);
                    break;
                case Plane.XY:
                    targetPos = target.value.transform.position.SetZ(0);
                    agentPos = agent.position.SetZ(0);
                    agentForward = agent.forward.SetZ(0);
                    break;
                case Plane.YZ:
                    targetPos = target.value.transform.position.SetX(0);
                    agentPos = agent.position.SetX(0);
                    agentForward = agent.forward.SetX(0);
                    break;
            }

            float angle = Vector3.Angle(targetPos - agentPos, agentForward);
            // Debug.Log($"angle: {angle}, targetPos: {targetPos}, agentPos: {agentPos}, agentForward: {agentForward}");
            if ( angle <= angleDifference.value )
            {
                EndAction();
                return;
            }

            var dir = targetPos - agentPos;
            agent.rotation = Quaternion.LookRotation(Vector3.RotateTowards(agentForward, dir, speed.value * Time.deltaTime, 0), upVector.value);  // axis wrong
            if ( !waitActionFinish )
            {
                EndAction();
            }
        }
    }
}