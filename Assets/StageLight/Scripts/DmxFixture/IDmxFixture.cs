using System;

namespace StageLight.DmxFixture
{
    public interface IDmxFixture
    {
        public string Name { get; set; }
        public int Universe { get; }
        public int StartAddress { get; }
        public int ChannelCount();

        public void UpdateValues(ReadOnlySpan<byte> values);
    }
}
