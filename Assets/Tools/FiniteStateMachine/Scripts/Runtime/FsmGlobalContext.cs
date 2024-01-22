// Requires .NET 4 or higher to make use of System.Lazy
using System.Collections.Generic;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    internal class FsmGlobalContext 
    {
        private static readonly System.Lazy<FsmGlobalContext> _lazy
            = new(() => new FsmGlobalContext());
        
        internal static FsmGlobalContext Instance => _lazy.Value;
        private readonly Dictionary<FsmStateMachine, IFsmRunner> _runners = new();
        
        private FsmGlobalContext()
        { }

        public bool HasRunner(FsmStateMachine stateMachine)
        {
            return _runners.ContainsKey(stateMachine);
        }

        public void RegisterRunner(IFsmRunner runner)
        {
            FsmStateMachine stateMachine = runner.StateMachine;
            
            if (!stateMachine)
                return;

            if (_runners.TryGetValue(stateMachine, out IFsmRunner currentRunner))
            {
                if (currentRunner == runner)
                    return;
                
                currentRunner.StateMachine = null;
            }
            
            _runners[stateMachine] = runner;
        }
    }
}