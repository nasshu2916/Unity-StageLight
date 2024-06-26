﻿using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StageLight.DmxFixture.Channels
{
    [RequireComponent(typeof(LightChannelManager))]
    public sealed class ZoomChannel : ChannelBase<float>, ISmoothChannel
    {
        [SerializeField] private float _minAngle = 15;
        [SerializeField] private float _maxAngle = 75;
        [SerializeField] private AnimationCurve _intensityZoomCurve = AnimationCurve.Linear(0, 0.99f, 1, 0.2f);
        [SerializeField] private bool _smooth = true;
        [SerializeField] private float _smoothTime = 0.1f;
        [SerializeField] private float _smoothMaxSpeed = float.PositiveInfinity;

        [SerializeField] private LightChannelManager _lightChannelManager;
        private float _currentVelocity;

        private float _targetAngle, _prevAngle;

        public override int ChannelSize { get; } = 1;

        private void Reset()
        {
            _lightChannelManager = GetComponent<LightChannelManager>();
        }

        public void Update()
        {
            if (!IsSmooth) return;

            var angle = Mathf.SmoothDamp(_prevAngle,
                _targetAngle,
                ref _currentVelocity,
                SmoothTime,
                SmoothMaxSpeed);

            ApplyAngle(angle);
        }

        public bool IsSmooth { get => _smooth; set => _smooth = value; }
        public float SmoothTime { get => _smoothTime; set => _smoothTime = value; }
        public float SmoothMaxSpeed { get => _smoothMaxSpeed; set => _smoothMaxSpeed = value; }

        protected override float CastValue(ReadOnlySpan<byte> values)
        {
            return values[0] / 255f;
        }

        protected override void UpdateChannel(float value)
        {
            _targetAngle = Mathf.Lerp(_minAngle, _maxAngle, value);
            if (IsSmooth) return;

            ApplyAngle(_targetAngle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyAngle(float angle)
        {
            _lightChannelManager.Angle = angle;
            _lightChannelManager.ZoomIntensity = _intensityZoomCurve.Evaluate((angle - _minAngle) / (_maxAngle - _minAngle));
            _prevAngle = angle;
        }
    }
}
