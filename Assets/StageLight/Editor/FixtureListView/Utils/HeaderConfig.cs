using System.Linq;
using UnityEditor.IMGUI.Controls;

namespace StageLight.FixtureListView
{
    internal static class HeaderConfig
    {
        private static readonly (MultiColumnHeaderState.Column column, ColumnType type)[] HeaderColumnAndType =
        {
            (new FixtureColumn("Fixture", 180f), ColumnType.FixtureName),
            (new FixtureColumn("ObjectName", 180f), ColumnType.ObjectName),
            (new FixtureColumn("Parent", 180f), ColumnType.ParentName),
            (new FixtureColumn("Universe", 80f), ColumnType.Universe),
            (new FixtureColumn("StartChannel", 80f), ColumnType.StartAddress),
            (new FixtureColumn("ChannelMode", 80f), ColumnType.ChannelMode)
        };

        public static readonly MultiColumnHeaderState.Column[] HeaderColumns =
            HeaderColumnAndType.Select(t => t.column).ToArray();

        public static readonly ColumnType[] HeaderColumnTypes = HeaderColumnAndType.Select(t => t.type).ToArray();

        /// <summary> ヘッダー列の個数 </summary>
        public static int HeaderColumnNum => HeaderColumnAndType.Length;


        public static float InitialHeaderTotalWidth
        {
            get
            {
                return HeaderColumns.Sum(t => t.width);
            }
        }
    }
}
