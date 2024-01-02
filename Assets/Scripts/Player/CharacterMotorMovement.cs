namespace Player
{
    // dnSpy decompiler from Assembly-CSharp.dll class: CharacterMotorMovement
    using System;
    using UnityEngine;

    [Serializable]
    public class CharacterMotorMovement
    {
        public float maxForwardSpeed = 264;

        public float maxSidewaysSpeed = 264;

        public float maxBackwardsSpeed = 264;

        public bool clampSpeedOnSlopes;

        public AnimationCurve maxSpeedOnSlope = new AnimationCurve(new Keyframe[]
        {
            new Keyframe(-90f, 10f),
            new Keyframe(0f, 10f),
            new Keyframe(90f, 0f)
        });

        public float maxGroundAcceleration = 100;

        public float maxGroundDecceration = 100;

        public float maxAirAcceleration = 100;

        public float maxAirDecceration = 100;

        public float gravity = 30f;

        public float maxFallSpeed = 30f;

        [NonSerialized]
        public CollisionFlags collisionFlags;

        [NonSerialized]
        public Vector3 velocity;

        [NonSerialized]
        public Vector3 frameVelocity = Vector3.zero;

        [NonSerialized]
        public Vector3 hitPoint = Vector3.zero;

        [NonSerialized]
        public Vector3 lastHitPoint = new Vector3(float.PositiveInfinity, 0f, 0f);
    }

}