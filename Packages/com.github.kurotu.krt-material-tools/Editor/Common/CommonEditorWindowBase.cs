
using UnityEditor;

namespace KRT.MaterialTools.Common
{
    internal abstract class CommonEditorWindowBase : EditorWindow
    {
        private void OnGUI()
        {
            if (UpdateChecker.ShouldNotifyUpdate())
            {
                UpdateChecker.NotifyUpdateGUI();
                EditorGUILayout.Space();
            }

            OnGUIInternal();
        }

        protected abstract void OnGUIInternal();
    }
}
