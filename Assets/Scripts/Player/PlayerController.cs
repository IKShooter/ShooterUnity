namespace Player
{
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5.0f;
        public float mouseSensitivity = 2.0f;
        public float jumpForce = 18.0f;
        public float gravity = 20.0f;
        public float crouchScale = 0.5f;

        private float verticalRotation = 0f;
        private CharacterController characterController;
        private bool isGrounded = false;
        private bool isCrouching = false;
        private Vector3 originalScale;

        private float lastNetworkUpdate = 0f;

        void Start()
        {
            characterController = GetComponent<CharacterController>();
            originalScale = transform.localScale;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            // Rotation
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivity;

            verticalRotation += mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

            transform.Rotate(Vector3.up * mouseX);
            Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

            // Movement
            float forwardSpeed = Input.GetAxis("Vertical") * moveSpeed;
            float sideSpeed = Input.GetAxis("Horizontal") * moveSpeed;

            Vector3 speed = new Vector3(sideSpeed, 0, forwardSpeed);
            speed = transform.rotation * speed;

            // Apply gravity
            if (characterController.isGrounded)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
                speed.y -= gravity * Time.deltaTime;
            }

            // Jumping
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                speed.y = jumpForce;
            }

            // Crouching
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                characterController.height *= crouchScale;
                transform.localScale = originalScale * crouchScale;
            }
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                characterController.height /= crouchScale;
                transform.localScale = originalScale;
            }

            characterController.Move(speed * Time.deltaTime);

            if (Time.time - lastNetworkUpdate > 0.1f)
            {
                NetworkManager.Instance.UpdatePlayer(transform.position, verticalRotation);
                lastNetworkUpdate = Time.time;
            }
        }
    }
}
