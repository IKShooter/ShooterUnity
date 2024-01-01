using UnityEngine;

namespace Player.Components
{
    public class MovementComponent : IControllerComponent
    {
        private float _walkingSpeed = 3.0f;
        private float _runningSpeed = 4.0f;
        private float _jumpForce = 5.0f;
        
        private float _crouchHeight = 0.5f;
        private float _originalHeight;
        private float _crouchSpeedMultiplier = 0.8f;
        
        private float _verticalVelocity;
        
        private readonly CharacterController _characterController;
        private readonly GameObject _body;

        private bool _isMoving;

        public bool IsMoving
        {
            get => _isMoving; 
        }

        public MovementComponent(CharacterController characterController, GameObject body)
        {
            _characterController = characterController;
            _originalHeight = _characterController.height;
            _body = body;
        }
        
        public void Update()
        {
            bool isGrounded = PlayerController.Instance.IsGrounded();

            // Player Movement
            float speed = Input.GetKey(KeyCode.LeftShift) ? _runningSpeed : _walkingSpeed;
            float horizontalMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            float verticalMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;

            Vector3 movement = _body.transform.right * horizontalMovement + _body.transform.forward * verticalMovement;
            _characterController.Move(movement);

            // Track is moving var
            _isMoving = horizontalMovement + verticalMovement != 0;

            // Apply gravity
            _verticalVelocity += (Physics.gravity.y / 3.5f) * Time.deltaTime;

            // Jump
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                _verticalVelocity = _jumpForce;
            }

            movement.y = _verticalVelocity * Time.deltaTime;

            CollisionFlags collisionFlags = _characterController.Move(movement);

            // Move with fix roof flying glitch
            if ((collisionFlags & CollisionFlags.CollidedAbove) != 0)
            {
                _verticalVelocity = 0;
                Vector3 ceilingDirection = Vector3.up;
                _characterController.Move(ceilingDirection * -0.1f);
            }
            
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                StartCrouch();
            } else if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                StopCrouch();
            }
        }
        
        void StartCrouch()
        {
            //_originalHeight = _characterController.height; // Store the original height
    
            // Reduce the height of the character controller
            _characterController.height = _crouchHeight;

            // Adjust the speed while crouching
            _walkingSpeed *= _crouchSpeedMultiplier;
            _runningSpeed *= _crouchSpeedMultiplier;
        }

        void StopCrouch()
        {
            // Restore the original height of the character controller
            _characterController.height = _originalHeight;

            // Reset the speed to normal values
            _walkingSpeed /= _crouchSpeedMultiplier;
            _runningSpeed /= _crouchSpeedMultiplier;
        }
    }
}