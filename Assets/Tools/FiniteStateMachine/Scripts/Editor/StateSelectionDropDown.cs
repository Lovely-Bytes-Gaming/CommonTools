using System;
using UnityEditor.IMGUI.Controls;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    public class StateSelectionDropDown : AdvancedDropdown
    {
        private class StateItem : AdvancedDropdownItem
        {
            public FsmState State;
            public StateItem(FsmState state) : base(state.name)
            {
                State = state;
            }
        }

        private Action<FsmState> _onSelect;
        
        public StateSelectionDropDown(AdvancedDropdownState state, Action<FsmState> onSelect) : base(state)
        {
            _onSelect = onSelect;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            FsmStateMachine[] stateMachines = EditorUtils.FindAssetsOfType<FsmStateMachine>();

            AdvancedDropdownItem root = new("State Machines");

            foreach (FsmStateMachine stateMachine in stateMachines)
            {
                AdvancedDropdownItem fsmItem = new(stateMachine.name);

                foreach (FsmState state in stateMachine.States)
                {
                    StateItem stateItem = new(state);
                    fsmItem.AddChild(stateItem);
                }
                
                root.AddChild(fsmItem);
            }
            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);

            if (item is not StateItem stateItem)
                return;
            
            _onSelect?.Invoke(stateItem.State);
        }
    }
}