﻿using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StageLight.DmxFixture.Channels
{
    [RequireComponent(typeof(LightChannelManager))]
    public sealed class ZoomChannel : ChannelBase<float>
    {

        [SerializeField] private float _minAngle = 15;
        [SerializeField] private float _maxAngle = 75;
        [SerializeField] private bool _smooth = true;
        [SerializeField] private float _smoothTime = 0.1f;
        [SerializeField] private float _smoothMaxSpeed = float.PositiveInfinity;

        [SerializeField] private LightChannelManager _lightChannelManager;

        private float _targetAngle, _prevAngle;
        private float _currentVelocity;

        public override int ChannelSize { get; } = 1;

        private void Reset()
        {
            _lightChannelManager = GetComponent<LightChannelManager>();
        }

        public void Update()
        {
            if (!_smooth) return;

            var angle = Mathf.SmoothDamp(_prevAngle,
                _targetAngle,
                ref _currentVelocity,
                _smoothTime,
                _smoothMaxSpeed);

            ApplyAngle(angle);
        }

        protected override float CastValue(ReadOnlySpan<byte> values)
        {
            return values[0] / 255f;
        }

        protected override void UpdateChannel(float value)
        {
            _targetAngle = Mathf.Lerp(_minAngle, _maxAngle, value);
            if (_smooth) return;

            ApplyAngle(_targetAngle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyAngle(float angle)
        {
            _lightChannelManager.Angle = angle;
            _prevAngle = angle;
        }
    }
}