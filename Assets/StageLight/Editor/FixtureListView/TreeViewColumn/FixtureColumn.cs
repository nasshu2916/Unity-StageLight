using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace StageLight.FixtureListView
{
    internal class FixtureColumn : MultiColumnHeaderState.Column
    {
        public FixtureColumn(string label, float width)
        {
            this.width = width;
            autoResize = false;
            headerContent = new GUIContent(label);
        }
    }
}
