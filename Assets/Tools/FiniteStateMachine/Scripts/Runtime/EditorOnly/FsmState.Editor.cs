#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    public partial class FsmState 
    {
        [Serializable]
        internal class ViewData
        {
            public string Guid;
            public Vector2 CanvasPosition;
        }
        
        [SerializeField, HideInInspector] 
        internal List<ViewData> Views = new();
    }
}

#endif