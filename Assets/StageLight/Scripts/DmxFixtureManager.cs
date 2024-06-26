using System;
using System.Collections.Generic;
using System.Linq;
using ArtNet;
using ArtNet.Packets;
using StageLight.DmxFixture;
using UnityEngine;

namespace StageLight
{
    public class DmxFixtureManager : MonoBehaviour
    {
        private readonly Queue<ushort> _updatedUniverses = new();
        private Dictionary<ushort, byte[]> DmxDictionary { get; } = new();
        public Dictionary<int, IEnumerable<IDmxFixture>> DmxDevices { get; private set; }

        public void Update()
        {
            lock (_updatedUniverses)
            {
                while (0 < _updatedUniverses.Count)
                {
                    var universe = _updatedUniverses.Dequeue();
                    ReadOnlySpan<byte> dmxValues = DmxDictionary[universe];
                    DmxDevices.TryGetValue(universe, out var devices);
                    if (devices == null) continue;
                    foreach (var device in devices)
                    {
                        var startAddress = device.StartAddress - 1;
                        var channelMode = device.ChannelMode;
                        if (dmxValues.Length < startAddress + channelMode) continue;
                        var values = dmxValues.Slice(startAddress, channelMode);
                        device.UpdateValues(values);
                    }
                }
            }
        }

        public void OnEnable()
        {
            Init();
        }

        private static Dictionary<int, IEnumerable<IDmxFixture>> FindDmxDevices()
        {
            return FindObjectsOfType<MonoBehaviour>().OfType<IDmxFixture>()
                .GroupBy(device => device.Universe)
                .ToDictionary(g => g.Key - 1, g => g as IEnumerable<IDmxFixture>);
        }

        [ContextMenu("Init")]
        public void Init()
        {
            DmxDevices = FindDmxDevices();
        }

        public void ReceivedDmxPacket(ReceivedData<DmxPacket> receivedData)
        {
            var packet = receivedData.Packet;
            var universe = packet.Universe;
            if (!DmxDictionary.ContainsKey(universe)) DmxDictionary.Add(universe, packet.Dmx);
            Buffer.BlockCopy(packet.Dmx, 0, DmxDictionary[universe], 0, 512);
            lock (_updatedUniverses)
            {
                if (_updatedUniverses.Contains(universe)) return;
                _updatedUniverses.Enqueue(universe);
            }
        }
    }
}
