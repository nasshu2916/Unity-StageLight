using System;

namespace StageLight.DmxFixture.Channels
{
    public abstract class ChannelBase<T> : DmxChannel where T : struct
    {
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                if (value.Equals(_value))
                {
                    return;
                }

                _value = value;
                UpdateChannel(value);
            }
        }

        protected abstract T CastValue(ReadOnlySpan<byte> values);

        protected abstract void UpdateChannel(T value);

        public override void UpdateValues(ReadOnlySpan<byte> values)
        {
            Value = CastValue(values);
        }
    }
}
