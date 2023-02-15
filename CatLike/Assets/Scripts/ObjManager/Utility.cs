using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    [System.Serializable]
    public struct FloatRange
    {
        public float min, max;

        public float RandomValueInRange
        {
            get
            {
                return Random.Range(min, max);
            }
        }
    }

    [System.Serializable]

    public struct SpawnConfiguration
    {
        public enum MovementDirection
        {
            Forward,
            Upward,
            Outward,
            Random
        }

        public MovementDirection movementDirection;

        public FloatRange speed;

        public FloatRange angularSpeed;

        public FloatRange scale;

        public ColorRangeHSV color;
    }

    [System.Serializable]
    public struct ColorRangeHSV
    {
        [FloatRangeSlider(0f, 1f)]
        public FloatRange hue, saturation, value;

        public Color RandomInRange
        {
            get
            {
                return Random.ColorHSV(
                    hue.min, hue.max,
                    saturation.min, saturation.max,
                    value.min, value.max,
                    1f, 1f);
            }
        }
    }
}
