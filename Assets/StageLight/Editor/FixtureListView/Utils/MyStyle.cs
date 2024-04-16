using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace StageLight.FixtureListView
{
    internal static class MyStyle
    {
        public static GUIStyle DefaultLabel => EditorStyles.label;

        public static GUIStyle TreeViewColumnHeader { get; } = new(MultiColumnHeader.DefaultStyles.columnHeader)
        {
            alignment = TextAnchor.LowerLeft
        };

        public static GUIStyle RedLabel { get; } = new(EditorStyles.label)
        {
            normal =
            {
                textColor = new Color(1f, 0f, 0f)
            }
        };
    }
}
