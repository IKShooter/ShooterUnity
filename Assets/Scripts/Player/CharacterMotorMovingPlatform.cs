namespace Player
{
    using System;
    using UnityEngine;

    public class CharacterMotorMovingPlatform
    {
        public bool enabled = true;

        public MovementTransferOnJump movementTransfer = MovementTransferOnJump.PermaTransfer;

        [NonSerialized]
        public Transform hitPlatform;

        [NonSerialized]
        public Transform activePlatform;

        [NonSerialized]
        public Vector3 activeLocalPoint;

        [NonSerialized]
        public Vector3 activeGlobalPoint;

        [NonSerialized]
        public Quaternion activeLocalRotation;

        [NonSerialized]
        public Quaternion activeGlobalRotation;

        [NonSerialized]
        public Matrix4x4 lastMatrix;

        [NonSerialized]
        public Vector3 platformVelocity;

        [NonSerialized]
        public bool newPlatform;
    }

}