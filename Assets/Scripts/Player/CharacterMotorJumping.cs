using System;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class CharacterMotorJumping
    {
        public bool enabled = true;

        public float baseHeight = 0.4f;

        public float extraHeight = 1.0f;

        public float perpAmount;

        public float steepPerpAmount = 0.5f;

        [NonSerialized]
        public bool jumping;

        [NonSerialized]
        public bool holdingJumpButton;

        [NonSerialized]
        public float lastStartTime;

        [NonSerialized]
        public float lastButtonDownTime = -100f;

        [NonSerialized]
        public Vector3 jumpDir = Vector3.up;
    }

}