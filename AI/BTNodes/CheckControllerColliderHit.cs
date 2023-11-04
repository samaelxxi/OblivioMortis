using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    [Category("System Events")]
    [Name("Check Controller Collider Hit")]
    public class CheckCollision_Controller : ConditionTask<CharacterController>
    {
        public bool specifiedTagOnly;
        [TagField]
        public string objectTag = "Untagged";


        protected override string info 
        {
            get { return "Controller Collider Hit " + ( specifiedTagOnly ? ( " '" + objectTag + "' tag" ) : "" ); }
        }

        protected override void OnEnable()
        {
            router.onControllerColliderHit += OnControllerColliderHit;
        }

        protected override void OnDisable()
        {
            router.onControllerColliderHit -= OnControllerColliderHit;
        }


        protected override bool OnCheck() 
        {
            return false;
        }

        public void OnControllerColliderHit(ParadoxNotion.EventData<ControllerColliderHit> data)
        {
            if ( !specifiedTagOnly || data.value.gameObject.CompareTag(objectTag) )
            {
                YieldReturn(true);
            }
        }
    }
}