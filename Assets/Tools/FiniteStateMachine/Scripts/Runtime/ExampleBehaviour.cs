using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [CreateAssetMenu(menuName = "LovelyBytes/CommonTools/FiniteStateMachine/ExampleBehaviour")]
    public class ExampleBehaviour : FsmBehaviour
    {
        public override void OnStart()
        {
            Debug.Log("Here we go!");
        }

        public override void OnStop()
        {
            Debug.Log("Goodbye!");
        }

        public override void OnUpdate(float deltaTime)
        {
            Debug.Log("Still standing");
        }
    }
}