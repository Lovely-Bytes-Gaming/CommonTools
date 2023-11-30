using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    [AddComponentMenu("LovelyBytes/CommonTools/FiniteStateMachine/FsmRunner")]
    public class FsmRunner : MonoBehaviour
    {
        [SerializeField] private FsmStateMachine _stateMachine;

        private void OnEnable()
        {
            _stateMachine.Enter();
        }

        private void Update()
        {
            _stateMachine.OnUpdate(Time.deltaTime);
        }

        private void OnDisable()
        {
            _stateMachine.Exit();
        }
    }
}
