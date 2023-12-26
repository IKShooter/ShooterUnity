namespace Player
{
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5.0f;
        public float mouseSensitivity = 2.0f;

        private float verticalRotation = 0f;
        private CharacterController characterController;

        private float lastNetworkUpdate = 0f;

        void Start()
        {
            characterController = GetComponent<CharacterController>();
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

            characterController.SimpleMove(speed);

            if (Time.time - lastNetworkUpdate > 0.5f)
            {
                NetworkManager.Instance.UpdatePlayer(transform.position, verticalRotation);
                lastNetworkUpdate = Time.time;
            }
        }
    }

}