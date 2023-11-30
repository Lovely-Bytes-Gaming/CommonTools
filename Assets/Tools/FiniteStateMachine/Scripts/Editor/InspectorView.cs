
using UnityEditor;
using UnityEngine.UIElements;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    public class InspectorView : VisualElement
    {
        private Editor _editor;
        
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        public void UpdateSelection(UnityEngine.Object selection)
        {
            Clear();

            if (!selection)
                return;
            
            UnityEngine.Object.DestroyImmediate(_editor);
            _editor = Editor.CreateEditor(selection);
            IMGUIContainer container = new(_editor.OnInspectorGUI);
            Add(container);
        }
    }
}