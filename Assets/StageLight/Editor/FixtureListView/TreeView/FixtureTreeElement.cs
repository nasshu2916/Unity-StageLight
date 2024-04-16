using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using StageLight.DmxFixture;
using UnityEngine;

namespace StageLight.FixtureListView
{
    internal class FixtureTreeElement
    {
        internal FixtureTreeElement(GameObject gameObject, IDmxFixture fixture)
        {
            GameObject = gameObject;
            Fixture = fixture;
        }
        public GameObject GameObject { get; }
        public IDmxFixture Fixture { get; }

        public int Id { get; set; }

        private string FixtureName => Fixture.Name;

        private string ObjectName => GameObject.name;

        private string ObjectParentName
        {
            get
            {
                var parent = GameObject.transform.parent;
                return parent == null ? "" : parent.name;
            }
        }

        private int Universe => Fixture.Universe;
        private int StartAddress => Fixture.StartAddress;
        private int ChannelMode => Fixture.ChannelMode;

        /// <summary> TreeViewのラベルのGUIStyle取得 </summary>
        public GUIStyle GetLabelStyle(ColumnType column)
        {
            var value = GetColumnValue(column);
            return (column, value) switch
            {
                (ColumnType.FixtureName, _) => MyStyle.DefaultLabel,
                (ColumnType.ObjectName, _) => MyStyle.DefaultLabel,
                (ColumnType.ParentName, _) => MyStyle.DefaultLabel,
                (ColumnType.Universe, int i) => i is <= 0 or > 512 ? MyStyle.RedLabel : MyStyle.DefaultLabel,
                (ColumnType.StartAddress, int i) => i is <= 0 or > 512 ? MyStyle.RedLabel : MyStyle.DefaultLabel,
                (ColumnType.ChannelMode, int i) => i is <= 0 or > 512 ? MyStyle.RedLabel : MyStyle.DefaultLabel,
                _ => throw new ArgumentOutOfRangeException(nameof(column), column, null)
            };
        }


        /// <summary> TreeViewで表示するテキスト取得 </summary>
        public string GetDisplayText(ColumnType column)
        {
            var value = GetColumnValue(column);
            return value switch
            {
                string s => s,
                int i => i.ToString(),
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Undefined value type")
            };
        }

        [NotNull]
        public object GetColumnValue(ColumnType column)
        {
            return column switch
            {
                ColumnType.FixtureName => FixtureName,
                ColumnType.ObjectName => ObjectName,
                ColumnType.ParentName => ObjectParentName,
                ColumnType.Universe => Universe,
                ColumnType.StartAddress => StartAddress,
                ColumnType.ChannelMode => ChannelMode,
                _ => throw new ArgumentOutOfRangeException(nameof(column), column, "Unknown column type")
            };
        }

        public Type GetColumnType(ColumnType column)
        {
            return column switch
            {
                ColumnType.FixtureName => typeof(string),
                ColumnType.ObjectName => typeof(string),
                ColumnType.ParentName => typeof(string),
                ColumnType.Universe => typeof(int),
                ColumnType.StartAddress => typeof(int),
                ColumnType.ChannelMode => typeof(int),
                _ => throw new ArgumentOutOfRangeException(nameof(column), column, null)
            };
        }

        /// <summary> 検索条件に一致するか </summary>
        public bool IsMatchSearch(IEnumerable<SearchState> searchStates)
        {
            return searchStates.Zip(HeaderConfig.HeaderColumnTypes,
                    (searchState, columnType) => (searchState, columnType))
                .Any(t => t.searchState.IsMatch(t.columnType, this));
        }
    }
}
