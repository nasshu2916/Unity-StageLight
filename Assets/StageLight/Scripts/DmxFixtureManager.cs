using System;
using System.Collections.Generic;
using System.Linq;
using ArtNet;
using ArtNet.Packets;
using UnityEngine;
using StageLight.DmxFixture;

namespace StageLight
{
    public class DmxFixtureManager : MonoBehaviour
    {
        private Dictionary<ushort, byte[]> DmxDictionary { get; } = new();
        private readonly Queue<ushort> _updatedUniverses = new();
        public Dictionary<int, IEnumerable<IDmxFixture>> DmxDevices { get; private set; }

        private static Dictionary<int, IEnumerable<IDmxFixture>> FindDmxDevices()
        {
            return FindObjectsOfType<MonoBehaviour>().OfType<IDmxFixture>()
                .GroupBy(device => device.Universe).ToDictionary(g => g.Key - 1, g => g as IEnumerable<IDmxFixture>);
        }

        [ContextMenu("Init")]
        public void Init()
        {
            DmxDevices = FindDmxDevices();
        }

        public void OnEnable()
        {
            Init();
        }

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
                        var dmxChannelCount = device.ChannelCount();
                        if (dmxValues.Length < startAddress + dmxChannelCount) continue;
                        var values = dmxValues.Slice(startAddress, dmxChannelCount);
                        device.UpdateValues(values);
                    }
                }
            }
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
