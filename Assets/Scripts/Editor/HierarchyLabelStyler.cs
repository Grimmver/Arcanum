#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace View.Editor
{
    [InitializeOnLoad]
    public class HierarchyLabelStyler
    {
        private static readonly Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        private static readonly Color textColor = Color.white;

        static HierarchyLabelStyler()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        }

        private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) return;

            string name = obj.name;

            if (name.StartsWith("="))
            {
                EditorGUI.DrawRect(selectionRect, backgroundColor);

                var style = new GUIStyle(EditorStyles.boldLabel)
                {
                    normal = { textColor = textColor },
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold,
                    fontSize = 11,
                    clipping = TextClipping.Clip,
                    wordWrap = false
                };

                string cleanName = name.Trim('=').Trim().ToUpperInvariant();

                EditorGUI.LabelField(selectionRect, cleanName.Trim('=').Trim(), style);
            }
        }
    }
}
#endif
