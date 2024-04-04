﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StageLight.DmxFixture.Channels
{
    public enum Axis
    {
        Pan,
        Tilt,
        Roll
    }

    public class RotationChannel : ChannelBase<float>
    {
        [SerializeField] private List<Transform> _rotationTarget = new();
        [SerializeField] private Axis _axis = Axis.Pan;

        [SerializeField] private float _minRotation = -180;
        [SerializeField] private float _maxRotation = 180;
        [SerializeField] private bool _invert = false;
        [SerializeField] private bool _smooth = true;
        [SerializeField] private float _smoothTime = 0.1f;
        [SerializeField] private float _smoothMaxSpeed = float.PositiveInfinity;

        private float _targetAngle, _prevAngle;
        private float _currentVelocity;

        public override int ChannelSize { get; } = 2;

        public void Update()
        {
            if (!_smooth) return;

            var angle = Mathf.SmoothDamp(_prevAngle,
                _targetAngle,
                ref _currentVelocity,
                _smoothTime,
                _smoothMaxSpeed);

            ApplyRotation(angle);
        }

        protected override float CastValue(ReadOnlySpan<byte> values)
        {
            return (values[0] << 8 | values[1]) / 65535f;
        }
        protected override void UpdateChannel(float value)
        {
            if (_rotationTarget == null) return;

            var collectedValue = Mathf.Lerp(_minRotation, _maxRotation, value);
            _targetAngle = _invert ? -collectedValue : collectedValue;

            if (_smooth) return;

            ApplyRotation(_targetAngle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyRotation(float angle)
        {
            var rotation = Quaternion.Euler(RotationVector * angle);
            foreach (var target in _rotationTarget.Where(target => target != null))
            {
                target.localRotation = rotation;
            }

            _prevAngle = angle;
        }

        private Vector3 RotationVector => _axis switch
        {
            Axis.Tilt => Vector3.right,
            Axis.Pan => Vector3.up,
            Axis.Roll => Vector3.forward,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
