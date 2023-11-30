using UnityEngine.UIElements;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
    }
}