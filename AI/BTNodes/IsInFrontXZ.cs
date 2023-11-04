using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    [Name("Target In View Angle XZ")]
    [Category("GameObject")]
    [Description("Checks whether the target is in the view angle of the agent")]
    public class IsInFrontXZ : ConditionTask<Transform>
    {

        [RequiredField]
        public BBParameter<GameObject> checkTarget;
        [SliderField(1, 180)]
        public BBParameter<float> viewAngle = 70f;

        protected override string info 
        {
            get { return checkTarget + " in view angle XZ"; }
        }

        protected override bool OnCheck()
        {
            return Vector3.Angle(checkTarget.value.transform.position.SetY(0) - agent.position.SetY(0), agent.forward.SetY(0)) < viewAngle.value;
            // return Vector2.Angle(checkTarget.value.transform.position.ToXZ() - (Vector2)agent.position.ToXZ(), agent.forward) < viewAngle.value;
        }

        // check if the target is in the view angle of the agent on the XZ plane

        public override void OnDrawGizmosSelected()
        {
            if ( agent != null ) 
            {
                Vector3 dir = agent.forward * 5;
                Vector3 left = Quaternion.Euler(0, -viewAngle.value * 0.5f, 0) * dir;
                Vector3 right = Quaternion.Euler(0, viewAngle.value * 0.5f, 0) * dir;
                Gizmos.DrawLine(agent.position, agent.position + left);
                Gizmos.DrawLine(agent.position, agent.position + right);
                Gizmos.DrawLine(agent.position + left, agent.position + right);
                // Gizmos.matrix = Matrix4x4.TRS(agent.position, agent.rotation, Vector3.one);
                // Gizmos.DrawFrustum(Vector3.zero, viewAngle.value, 5, 0, 1f);
            }
        }

    }
}