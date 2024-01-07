using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class CharacterMotor
    {
        public CharacterMotorJumping jumping = new CharacterMotorJumping();

        public CharacterMotorMovingPlatform movingPlatform = new CharacterMotorMovingPlatform();

        public CharacterMotorMovement movement = new CharacterMotorMovement();

        public CharacterMotorSliding sliding = new CharacterMotorSliding();

        public bool canControl = true;

        public bool useFixedUpdate = true;

        public Vector3 explosionForce = Vector3.zero;

        private float explosionSpeed = 1f;

        [NonSerialized]
        public Vector3 inputMoveDirection = Vector3.zero;

        [NonSerialized]
        public bool inputJump;
        
        public bool IsMoving => false;

        [NonSerialized]
        public bool grounded = true;

        [NonSerialized]
        public Vector3 groundNormal = Vector3.zero;

        private Vector3 lastGroundNormal = Vector3.zero;

        private Transform tr;

        private GameObject _body;

        private CharacterController controller;

        //private MotorSecurity motorSecurity;

        private object lockMotorSecurity = new object();

        private int numMotorSecurityAlerts;

        private float nextCheck;

        private float altitude;

        private float lastStartTime = -1f;

        private int lstccounter;

        private Vector3 lastPosition = Vector3.zero;

        public Vector3 reportExplosionForce = Vector3.zero;

        public float reportMaxSpeedInDirection;

        public Vector3 reportDesiredLocalDirection = Vector3.zero;

        public float reportMaxSidewaySpeed;
        
 //        public int Check()
	// {
	// 	return this.motorSecurity.Check(this);
	// }

	public CharacterMotor(GameObject body, CharacterController characterController)
	{
		_body = body;
		
		object obj = this.lockMotorSecurity;
		lock (obj)
		{
			//this.motorSecurity = new MotorSecurity(this);
		}
		controller = characterController;
		tr = _body.transform;
	}

	public float Altitude
	{
		get
		{
			return this.altitude;
		}
	}

	public int LSTCCounter
	{
		get
		{
			return this.lstccounter;
		}
	}

	public float CalculateAltitude()
	{
		int num = 772;
		num = ~num;
		Vector3 vector = _body.transform.position + _body.transform.localToWorldMatrix.MultiplyVector(this.controller.center);
		Vector3 point = vector;
		this.altitude = 1000f;
		RaycastHit raycastHit;
		if (Physics.CapsuleCast(vector, point, this.controller.radius + 1f, Vector3.down, out raycastHit, 1000f, num))
		{
			this.altitude = raycastHit.distance - 2.55f;
		}
		return this.altitude;
	}

	private void UpdateFunction()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
		{
			this.lastStartTime = this.jumping.lastStartTime;
			this.lstccounter = 0;
		}
		else if (UnityEngine.Input.GetKey(KeyCode.Space))
		{
			if (this.lastStartTime != this.jumping.lastStartTime && !this.grounded)
			{
				this.lastStartTime = this.jumping.lastStartTime;
				this.lstccounter++;
			}
			else
			{
				this.lstccounter = 0;
			}
		}
		else
		{
			this.lstccounter = 0;
		}
		if (PlayerController.Instance != null && !PlayerController.Instance.IsEnabled())
		{
			return;
		}
		if (Time.time > this.nextCheck)
		{
			// float num = (float)this.Check();
			// if (num != 0f)
			// {
			// 	UnityEngine.Debug.Log(string.Format("Altitude Speed: {0}", (int)(200f * num)));
			// }
			this.nextCheck = Time.time + 5f;
		}
		Vector3 vector = this.movement.velocity;
		vector = this.ApplyInputVelocityChange(vector);
		if (this.explosionForce.x > this.reportExplosionForce.x)
		{
			this.reportExplosionForce.x = this.explosionForce.x;
		}
		if (this.explosionForce.y > this.reportExplosionForce.y)
		{
			this.reportExplosionForce.y = this.explosionForce.y;
		}
		if (this.explosionForce.z > this.reportExplosionForce.z)
		{
			this.reportExplosionForce.z = this.explosionForce.z;
		}
		vector = this.ApplyGravityAndJumping(vector);
		vector = this.ApplyExplosionForces(vector);
		Vector3 vector2 = Vector3.zero;
		if (this.MoveWithPlatform())
		{
			Vector3 a = this.movingPlatform.activePlatform.TransformPoint(this.movingPlatform.activeLocalPoint);
			vector2 = a - this.movingPlatform.activeGlobalPoint;
			if (vector2 != Vector3.zero)
			{
				this.controller.Move(vector2);
			}
			Quaternion lhs = this.movingPlatform.activePlatform.rotation * this.movingPlatform.activeLocalRotation;
			float y = (lhs * Quaternion.Inverse(this.movingPlatform.activeGlobalRotation)).eulerAngles.y;
			if (y != 0f)
			{
				this.tr.Rotate(0f, y, 0f);
			}
		}
		Vector3 vector3 = vector * Time.deltaTime;
		float stepOffset = this.controller.stepOffset;
		Vector3 vector4 = new Vector3(vector3.x, 0f, vector3.z);
		float d = Mathf.Max(stepOffset, vector4.magnitude);
		if (this.grounded)
		{
			vector3 -= d * Vector3.up;
		}
		this.movingPlatform.hitPlatform = null;
		this.groundNormal = Vector3.zero;
		this.movement.collisionFlags = this.controller.Move(vector3);
		this.movement.lastHitPoint = this.movement.hitPoint;
		this.lastGroundNormal = this.groundNormal;
		if (this.movingPlatform.enabled && this.movingPlatform.activePlatform != this.movingPlatform.hitPlatform && this.movingPlatform.hitPlatform != null)
		{
			this.movingPlatform.activePlatform = this.movingPlatform.hitPlatform;
			this.movingPlatform.lastMatrix = this.movingPlatform.hitPlatform.localToWorldMatrix;
			this.movingPlatform.newPlatform = true;
		}
		this.movement.velocity = vector;
		if ((double)this.movement.velocity.y < (double)vector.y - 0.001)
		{
			if (this.movement.velocity.y < 0f)
			{
				this.movement.velocity.y = vector.y;
			}
			else
			{
				this.jumping.holdingJumpButton = false;
			}
		}
		if (this.grounded && !this.IsGroundedTest())
		{
			this.grounded = false;
			if (this.movingPlatform.enabled && (this.movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer || this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
			{
				this.movement.frameVelocity = this.movingPlatform.platformVelocity;
				this.movement.velocity += this.movingPlatform.platformVelocity;
			}
			PlayerController.Instance.SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
			this.tr.position += d * Vector3.up;
		}
		else if (!this.grounded && this.IsGroundedTest())
		{
			this.grounded = true;
			this.jumping.jumping = false;
			this.SubtractNewPlatformVelocity();
			PlayerController.Instance.SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
		}
		if (this.MoveWithPlatform())
		{
			this.movingPlatform.activeGlobalPoint = this.tr.position + Vector3.up * (this.controller.center.y - this.controller.height * 0.5f + this.controller.radius);
			this.movingPlatform.activeLocalPoint = this.movingPlatform.activePlatform.InverseTransformPoint(this.movingPlatform.activeGlobalPoint);
			this.movingPlatform.activeGlobalRotation = this.tr.rotation;
			this.movingPlatform.activeLocalRotation = Quaternion.Inverse(this.movingPlatform.activePlatform.rotation) * this.movingPlatform.activeGlobalRotation;
		}
		this.lastPosition = _body.transform.position;
	}

	private void FixedUpdate()
	{
		if (this.grounded)
		{
		}
		if (this.movingPlatform.enabled)
		{
			if (this.movingPlatform.activePlatform != null)
			{
				if (!this.movingPlatform.newPlatform)
				{
					Vector3 platformVelocity = this.movingPlatform.platformVelocity;
					this.movingPlatform.platformVelocity = (this.movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocalPoint) - this.movingPlatform.lastMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocalPoint)) / Time.deltaTime;
				}
				this.movingPlatform.lastMatrix = this.movingPlatform.activePlatform.localToWorldMatrix;
				this.movingPlatform.newPlatform = false;
			}
			else
			{
				this.movingPlatform.platformVelocity = Vector3.zero;
			}
		}
		if (this.useFixedUpdate)
		{
			this.UpdateFunction();
		}
	}
	
	public Vector3 moveDir;

	public void Update()
	{
		FixedUpdate();
	}

	private Vector3 ApplyInputVelocityChange(Vector3 velocity)
	{
		if (!this.canControl)
		{
			this.inputMoveDirection = Vector3.zero;
		}
		this.IsSliding();
		Vector3 vector = this.GetDesiredHorizontalVelocity();
		if (this.movingPlatform.enabled && this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
		{
			vector += this.movement.frameVelocity;
			vector.y = 0f;
		}
		if (this.grounded)
		{
			vector = this.AdjustGroundVelocityToNormal(vector, this.groundNormal);
		}
		else
		{
			velocity.y = 0f;
		}
		Vector3 b = vector - velocity;
		float num = this.GetMaxAcceleration(this.grounded, vector.sqrMagnitude - velocity.sqrMagnitude) * Time.deltaTime;
		if (b.sqrMagnitude > num * num)
		{
			b = b.normalized * num;
		}
		if (this.grounded || this.canControl)
		{
			velocity += b;
		}
		if (this.grounded)
		{
			velocity.y = Mathf.Min(velocity.y, 0f);
		}
		return velocity;
	}

	private Vector3 ApplyExplosionForces(Vector3 velocity)
	{
		if (this.explosionForce == Vector3.zero)
		{
			return velocity;
		}
		velocity.x += this.explosionForce.x;
		velocity.z += this.explosionForce.z;
		if (this.grounded)
		{
			this.grounded = false;
			if (this.explosionForce.y > 0f)
			{
				velocity.y += this.explosionForce.y;
			}
		}
		else
		{
			velocity.y += this.explosionForce.y;
		}
		this.explosionForce = new Vector3(0f, 0f, 0f);
		return velocity;
	}

	public void Reset()
	{
		this.movement.velocity = Vector3.zero;
		this.inputMoveDirection = Vector3.zero;
		this.movement.frameVelocity = Vector3.zero;
		Input.ResetInputAxes();
	}

	public void SetExplosionForce(Vector3 ef, float power)
	{
		this.explosionForce = _body.transform.position;
		this.explosionForce -= ef;
		this.explosionForce.Normalize();
		this.explosionForce = this.explosionForce * power * this.explosionSpeed;
	}

	public void LiftUp()
	{
		movement.velocity = new Vector3(0, 7, 0);
	}

	public Vector3 ApplyGravityAndJumping(Vector3 velocity)
	{
		if (!this.inputJump || !this.canControl)
		{
			this.jumping.holdingJumpButton = false;
			this.jumping.lastButtonDownTime = -100f;
		}
		if (this.inputJump && this.jumping.lastButtonDownTime < 0f && this.canControl)
		{
			this.jumping.lastButtonDownTime = Time.time;
		}
		if (this.grounded)
		{
			velocity.y = Mathf.Min(0f, velocity.y) - this.movement.gravity * Time.deltaTime;
		}
		else
		{
			velocity.y = this.movement.velocity.y - this.movement.gravity * Time.deltaTime;
			if (this.jumping.jumping && this.jumping.holdingJumpButton && Time.time < this.jumping.lastStartTime + this.jumping.extraHeight / this.CalculateJumpVerticalSpeed(this.jumping.baseHeight))
			{
				velocity += this.jumping.jumpDir * this.movement.gravity * Time.deltaTime;
			}
			velocity.y = Mathf.Max(velocity.y, -this.movement.maxFallSpeed);
		}
		if (this.grounded)
		{
			if (this.jumping.enabled && this.canControl && (double)(Time.time - this.jumping.lastButtonDownTime) < 0.2)
			{
				this.grounded = false;
				this.jumping.jumping = true;
				this.jumping.lastStartTime = Time.time;
				this.jumping.lastButtonDownTime = -100f;
				this.jumping.holdingJumpButton = true;
				if (this.TooSteep())
				{
					this.jumping.jumpDir = Vector3.Slerp(Vector3.up, this.groundNormal, this.jumping.steepPerpAmount);
				}
				else
				{
					this.jumping.jumpDir = Vector3.Slerp(Vector3.up, this.groundNormal, this.jumping.perpAmount);
				}
				velocity.y = 0f;
				var a = this.jumping.jumpDir * this.CalculateJumpVerticalSpeed(this.jumping.baseHeight);
				velocity += a;
				if (this.movingPlatform.enabled && (this.movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer || this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
				{
					this.movement.frameVelocity = this.movingPlatform.platformVelocity;
					velocity += this.movingPlatform.platformVelocity;
				}
				PlayerController.Instance.SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				this.jumping.holdingJumpButton = false;
			}
		}
		return velocity;
	}

	public void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Vector3 vector = (hit.collider.GetType() != Type.GetType("BoxCollider")) ? hit.normal : Vector3.up;
		if (vector.y > 0f && vector.y > this.groundNormal.y && hit.moveDirection.y < 0f)
		{
			if ((double)(hit.point - this.movement.lastHitPoint).sqrMagnitude > 0.001 || this.lastGroundNormal == Vector3.zero)
			{
				this.groundNormal = vector;
			}
			else
			{
				this.groundNormal = this.lastGroundNormal;
			}
			this.movingPlatform.hitPlatform = hit.collider.transform;
			this.movement.hitPoint = hit.point;
			this.movement.frameVelocity = Vector3.zero;
		}
	}

	private void SubtractNewPlatformVelocity()
	{
	}

	private bool MoveWithPlatform()
	{
		return this.movingPlatform.enabled && (this.grounded || this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked) && this.movingPlatform.activePlatform != null;
	}

	private Vector3 GetDesiredHorizontalVelocity()
	{
		Vector3 vector = this.tr.InverseTransformDirection(this.inputMoveDirection);
		float num = this.MaxSpeedInDirection(vector);
		if (num > this.reportMaxSpeedInDirection)
		{
			this.reportMaxSpeedInDirection = num;
		}
		if (this.movement.clampSpeedOnSlopes && this.grounded && (double)this.movement.velocity.sqrMagnitude > (double)(num * num) * 0.0001)
		{
			float num2 = Mathf.Asin(this.movement.velocity.normalized.y) * 57.29578f;
			float num3 = Mathf.Acos(this.groundNormal.y) * 57.29578f;
			if (Mathf.Abs(num2) > num3)
			{
				num2 = num3 * Mathf.Sign(num2);
			}
			num = Mathf.Clamp(num, 0f, this.movement.maxSpeedOnSlope.Evaluate(num2));
		}
		if (vector.x > this.reportDesiredLocalDirection.x)
		{
			this.reportDesiredLocalDirection.x = vector.x;
		}
		if (vector.y > this.reportDesiredLocalDirection.y)
		{
			this.reportDesiredLocalDirection.y = vector.y;
		}
		if (vector.z > this.reportDesiredLocalDirection.z)
		{
			this.reportDesiredLocalDirection.z = vector.z;
		}
		return this.tr.TransformDirection(vector * num);
	}

	private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
	{
		Vector3 lhs = Vector3.Cross(Vector3.up, hVelocity);
		return Vector3.Cross(lhs, groundNormal).normalized * hVelocity.magnitude;
	}

	private bool IsGroundedTest()
	{
		return (double)this.groundNormal.y > 0.01;
	}

	private float GetMaxAcceleration(bool grounded, float acceleration)
	{
		if (grounded)
		{
			if (acceleration < 0f)
			{
				return this.movement.maxGroundAcceleration;
			}
			return this.movement.maxGroundDecceration;
		}
		else
		{
			if (acceleration < 0f)
			{
				return this.movement.maxAirAcceleration;
			}
			return this.movement.maxAirDecceration;
		}
	}

	public float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt(2f * targetJumpHeight * this.movement.gravity);
	}

	private bool IsJumping()
	{
		return this.jumping.jumping;
	}

	private bool IsSliding()
	{
		return this.grounded && this.sliding.enabled && this.TooSteep();
	}

	private bool IsTouchingCeiling()
	{
		return (this.movement.collisionFlags & CollisionFlags.Above) != CollisionFlags.None;
	}

	private bool IsGrounded()
	{
		return this.grounded;
	}

	private bool TooSteep()
	{
		return this.groundNormal.y <= Mathf.Cos(this.controller.slopeLimit * 0.0174532924f);
	}

	private Vector3 GetDirection()
	{
		return this.inputMoveDirection;
	}

	private void SetControllable(bool controllable)
	{
		this.canControl = controllable;
	}

	private float MaxSpeedInDirection(Vector3 desiredMovementDirection)
	{
		if (desiredMovementDirection == Vector3.zero)
		{
			return 0f;
		}
		if (this.reportMaxSidewaySpeed < this.movement.maxSidewaysSpeed)
		{
			this.reportMaxSidewaySpeed = this.movement.maxSidewaysSpeed;
		}
		float num = ((desiredMovementDirection.z <= 0f) ? this.movement.maxBackwardsSpeed : this.movement.maxForwardSpeed) / this.movement.maxSidewaysSpeed;
		Vector3 vector = new Vector3(desiredMovementDirection.x, 0f, desiredMovementDirection.z / num);
		Vector3 normalized = vector.normalized;
		Vector3 vector2 = new Vector3(normalized.x, 0f, normalized.z * num);
		return vector2.magnitude * this.movement.maxSidewaysSpeed;
	}

	private void SetVelocity(Vector3 velocity)
	{
		this.grounded = false;
		this.movement.velocity = velocity;
		this.movement.frameVelocity = Vector3.zero;
		PlayerController.Instance.SendMessage("OnExternalVelocity");
	}
    }
}