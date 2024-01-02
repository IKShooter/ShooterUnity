#define USE_CROUCH

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Components
{
    public class MovementComponent : IControllerComponent
    {
        public bool IsMoving => false;
        
        public float runSpeed = 6.6f;

        public float runStrafeSpeed = 6.07f;

        public float walkSpeed = 5.0f;

        public float walkStrafeSpeed = 5.0f;

        public float crouchRunSpeed = 3f;

        public float crouchRunStrafeSpeed = 3f;

        public float crouchWalkSpeed = 1.8f;

        public float crouchWalkStrafeSpeed = 1.8f;

        //public Camera FPSCamera;

        //private FPSCamera FPSCameraFPS;

        //public Camera TPSCamera;

        //public Camera WeaponCamera;

        public GameObject radarObject;

        //public float maxRotationSpeed = 540f;

        //public float zoomFactor = 1f;

        //public float minCarDistance;

        //public static bool dead;

        public bool walk;

        public bool oldwalk;

        public bool crouch;

        private bool oldcrouch;

        public bool inAir;

        public bool oldinAir;

        public long lastAir;

        public long lastAirBan;

        public bool fire;

        public bool aim;

        public bool oldaim;

        public bool reloading;

        public string currentWeaponName;

        public int currentWeapon;

        public bool grounded;

        public float targetYRotation;

        public float targetXRotation;

        private Transform soldierTransform;

        private CharacterController controller;

        private CharacterMotor motor;

        //private CombatPlayer player;

        private bool firing;

        private bool oldfiring;

        private float firingTimer;

        public float idleTimer;

        private float horizontalAxis;

        private float verticalAxis;

        private bool customAxisControl = true;

        public float customAxisAcceleration = 0.05f;

        public Vector3 moveDir;

        private bool _useIK;

        private float lastStartTime = -1f;

        private int lstccounter;

        private GameObject _body;
        
        public MovementComponent(GameObject body, CharacterController characterController)
        {
            _body = body;
            motor = new CharacterMotor(body, characterController);
           this.controller = characterController;
            
            Start();
        }

        public void Reset()
        {
            this.horizontalAxis = 0f;
            this.verticalAxis = (this.horizontalAxis);
        }

        private void Start()
        {
            this.idleTimer = 0f;
            this.soldierTransform = _body.transform;
            this.walk = false;
            this.aim = false;
            this.reloading = false;
        }

        private void OnEnable()
        {
            if (this.radarObject != null)
            {
                this.radarObject.SetActiveRecursively(true);
            }

            this.moveDir = Vector3.zero;
            this.walk = false;
            this.aim = false;
            this.reloading = false;
            this.inAir = false;
            this.lastAir = TimeManager.NetworkTime;
            this.lastAirBan = (this.lastAir);
        }

        private void OnDisable()
        {
            if (this.radarObject != null)
            {
                this.radarObject.SetActiveRecursively(false);
            }

            this.moveDir = Vector3.zero;
            this.walk = false;
            this.aim = false;
            this.reloading = false;
            this.inAir = false;
            this.lastAir = TimeManager.NetworkTime;
        }

        public int LSTCCounter
        {
            get { return this.lstccounter; }
        }

        public void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                this.lastStartTime = this.motor.jumping.lastStartTime;
                this.lstccounter = 0;
            }
            else if (UnityEngine.Input.GetKey(KeyCode.Space))
            {
                if (this.lastStartTime != this.motor.jumping.lastStartTime && !this.motor.grounded)
                {
                    this.lastStartTime = this.motor.jumping.lastStartTime;
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

            // if (GameHUD.IsShowMenu)
            // {
            //     return;
            // }

            this.GetUserInputs();
            if (!this.motor.canControl)
            {
                this.motor.canControl = true;
            }

            if (PlayerController.Instance.NetworkSyncComponent.IsAlive)
            {
                if (this.customAxisControl)
                {
                    this.moveDir = new Vector3(this.horizontalAxis, 0f, this.verticalAxis);
                }
                else
                {
                    this.moveDir = new Vector3(UnityEngine.Input.GetAxis("Horizontal"), 0f,
                        UnityEngine.Input.GetAxis("Vertical"));
                }
            }
            else
            {
                this.moveDir = Vector3.zero;
                this.motor.canControl = false;
            }

            if (this.moveDir.sqrMagnitude > 1f)
            {
                this.moveDir = this.moveDir.normalized;
            }

            this.motor.inputMoveDirection = _body.transform.TransformDirection(this.moveDir);
            this.motor.inputJump = (UnityEngine.Input.GetKey(KeyCode.Space) && !this.crouch);
            this.motor.movement.maxForwardSpeed = ((!this.walk)
                ? ((!this.crouch) ? this.runSpeed : this.crouchRunSpeed)
                : ((!this.crouch) ? this.walkSpeed : this.crouchWalkSpeed));
            if (this.walk != this.oldwalk)
            {
                //base.SendMessage("SetWalk", this.walk, SendMessageOptions.DontRequireReceiver);
                this.oldwalk = this.walk;
            }

            if (this.crouch != this.oldcrouch)
            {
                //base.SendMessage("SetCrouch", this.crouch, SendMessageOptions.DontRequireReceiver);
                this.idleTimer = 0f;
                this.oldcrouch = this.crouch;
            }

            // if (PlayerController.Instance.RoomSettings.GameMode == MapMode.MODE.ZOMBIE &&
            //     PlayerManager.Instance.LocalPlayer.Team == 1)
            // {
            //     this.motor.movement.maxGroundAcceleration = 1000f;
            //     this.motor.movement.maxGroundDecceration = 1000f;
            //     this.motor.movement.maxAirAcceleration = 70f;
            //     this.motor.movement.maxAirDecceration = 400f;
            //     this.motor.jumping.baseHeight = 6f;
            this.motor.movement.maxBackwardsSpeed = this.motor.movement.maxForwardSpeed;
            this.motor.movement.maxSidewaysSpeed = ((!this.walk)
                ? ((!this.crouch) ? this.runStrafeSpeed : this.crouchRunStrafeSpeed)
                : ((!this.crouch) ? this.walkStrafeSpeed : this.crouchWalkStrafeSpeed));
            //     float num = 1.3f;
            //     if (PlayerManager.Instance.LocalPlayer.ZombieType == ZombieType.Boss)
            //     {
            //         num = 1.265f;
            //     }
            //
            //     if (ShotController.Instance != null && ShotController.Instance.CurrentWeapon != null && num != 1f)
            //     {
            //         this.motor.movement.maxForwardSpeed = this.motor.movement.maxForwardSpeed * num;
            //         this.motor.movement.maxBackwardsSpeed = this.motor.movement.maxBackwardsSpeed * num;
            //         this.motor.movement.maxSidewaysSpeed = this.motor.movement.maxSidewaysSpeed;
            //     }
            // }
            // else
            // {
                // this.motor.movement.maxGroundAcceleration = 360f;
                // this.motor.movement.maxGroundDecceration = 100f;
                // this.motor.movement.maxAirAcceleration = 10f;
                // this.motor.movement.maxAirDecceration = 10f;
                // this.motor.jumping.baseHeight = 2f;
                // this.motor.movement.maxBackwardsSpeed = this.motor.movement.maxForwardSpeed;
                // this.motor.movement.maxSidewaysSpeed = ((!this.walk)
                //     ? ((!this.crouch) ? this.runStrafeSpeed : this.crouchRunStrafeSpeed)
                //     : ((!this.crouch) ? this.walkStrafeSpeed : this.crouchWalkStrafeSpeed));
                //float speedMultiplier = ShotController.Instance.GetSpeedMultiplier();
                // if (!ShotController.Instance.CheckSpeedMultiplier(speedMultiplier))
                // {
                //     PlayerController.Instance.SendEnterBaseRequest(5);
                // }

                // TODO: !!!
                // if (PlayerController.Instance.NetworkSyncComponent.CurrentWeapon != null &&
                //     speedMultiplier != 1f)
                // {
                //     this.motor.movement.maxForwardSpeed = this.motor.movement.maxForwardSpeed * speedMultiplier;
                //     this.motor.movement.maxBackwardsSpeed = this.motor.movement.maxBackwardsSpeed * speedMultiplier;
                //     this.motor.movement.maxSidewaysSpeed = this.motor.movement.maxSidewaysSpeed * speedMultiplier;
                // }
            //}

            if (this.moveDir != Vector3.zero)
            {
                this.idleTimer = 0f;
            }

            this.inAir = !this.motor.grounded;
            if (this.oldinAir != this.inAir)
            {
                //base.SendMessage("SetInAir", this.inAir, SendMessageOptions.DontRequireReceiver);
                this.oldinAir = this.inAir;
                this.lastAir = TimeManager.NetworkTime;
            }
            else if (this.inAir && TimeManager.NetworkTime - this.lastAir > 1700L)
            {
                //EffectManager.Instance.FallDownEffect(PlayerManager.Instance.LocalPlayer);
                this.lastAir = TimeManager.NetworkTime;
            }
            
            this.motor.Update();
        }

        private void GetUserInputs()
        {
            if (UnityEngine.Input.GetKey(KeyCode.LeftArrow) || UnityEngine.Input.GetKey(KeyCode.A))
            {
                if (this.horizontalAxis > 0f)
                {
                    this.horizontalAxis = 0f;
                }

                if (this.horizontalAxis > -1f + this.customAxisAcceleration / 2f)
                {
                    this.horizontalAxis -= this.customAxisAcceleration;
                }
            }
            else if (UnityEngine.Input.GetKey(KeyCode.RightArrow) || UnityEngine.Input.GetKey(KeyCode.D))
            {
                if (this.horizontalAxis < 0f)
                {
                    this.horizontalAxis = 0f;
                }

                if (this.horizontalAxis < 1f - this.customAxisAcceleration / 2f)
                {
                    this.horizontalAxis += this.customAxisAcceleration;
                }
            }
            else if (this.horizontalAxis > this.customAxisAcceleration / 2f)
            {
                this.horizontalAxis -= this.customAxisAcceleration;
            }
            else if (this.horizontalAxis < -this.customAxisAcceleration / 2f)
            {
                this.horizontalAxis += this.customAxisAcceleration;
            }

            if (UnityEngine.Input.GetKey(KeyCode.UpArrow) || UnityEngine.Input.GetKey(KeyCode.W))
            {
                if (this.verticalAxis < 0f)
                {
                    this.verticalAxis = 0f;
                }

                if (this.verticalAxis < 1f - this.customAxisAcceleration / 2f)
                {
                    this.verticalAxis += this.customAxisAcceleration;
                }
            }
            else if (UnityEngine.Input.GetKey(KeyCode.DownArrow) || UnityEngine.Input.GetKey(KeyCode.S))
            {
                if (this.verticalAxis > 0f)
                {
                    this.verticalAxis = 0f;
                }

                if (this.verticalAxis > -1f + this.customAxisAcceleration / 2f)
                {
                    this.verticalAxis -= this.customAxisAcceleration;
                }
            }
            else if (this.verticalAxis > this.customAxisAcceleration / 2f)
            {
                this.verticalAxis -= this.customAxisAcceleration;
            }
            else if (this.verticalAxis < -this.customAxisAcceleration / 2f)
            {
                this.verticalAxis += this.customAxisAcceleration;
            }

            // this.aim = (Input.GetButton("Fire2") && PlayerController.Instance.NetworkSyncComponent.IsAlive);
            // if (this.oldaim != this.aim)
            // {
            //     //base.SendMessage("SetAim", this.aim, SendMessageOptions.DontRequireReceiver);
            //     this.oldaim = this.aim;
            // }

            this.idleTimer += Time.deltaTime;
            if (this.aim || this.fire)
            {
                this.firingTimer -= Time.deltaTime;
                this.idleTimer = 0f;
            }
            else
            {
                this.firingTimer = 0.3f;
            }

            this.firing = (this.firingTimer <= 0f && this.fire);
            if (UnityEngine.Input.GetKey(KeyCode.LeftControl))
            {
                if (!this.crouch)
                {
                    this.crouch = true;
                }
            }
            else if (this.crouch)
            {
                this.crouch = false;
            }

            this.crouch |= !PlayerController.Instance.NetworkSyncComponent.IsAlive;
            this.walk = ((UnityEngine.Input.GetKey(KeyCode.LeftShift) && PlayerController.Instance.NetworkSyncComponent.IsAlive) ||
                         this.moveDir == Vector3.zero || this.crouch);
            // if (ShotController.Instance.CurrentWeapon == null)
            // {
            //     return;
            // }

            // this.walk = (this.walk || (ShotController.Instance.CurrentWeapon.Type == WeaponType.GATLING_GUN &&
            //                            (ShotController.Instance.IsShooting || ShotController.Instance.IsShootingAlt)));
            // this.walk = (this.walk || (ShotController.Instance.CurrentWeapon.Type == WeaponType.SNIPER_RIFLE &&
            //                            ShotController.Instance.Zoom));
            // this.walk = (this.walk || (this.player.Flame.Material == null && this.player.Flame.On));
        }

        public void OnControllerColliderHit(ControllerColliderHit hit)
        {
            motor.OnControllerColliderHit(hit);
        }
    }
}