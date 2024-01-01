using UnityEngine;

namespace Player.Components
{
    public class MouseControlComponent : IControllerComponent
    {
        private float _mouseSensitivity = 3.0f;
        private float _verticalRotation = 0f;

        private readonly Camera _playerCamera;
        private readonly GameObject _body;

        public MouseControlComponent(Camera playerCamera, GameObject body)
        {
            _playerCamera = playerCamera;
            _body = body;
        }

        public void Update()
        {
            // Player Look
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;// * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;// * Time.deltaTime;

            _verticalRotation -= mouseY;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -90f, 90f);
        
            _playerCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
            _body.transform.Rotate(Vector3.up * mouseX);
        }
    }
}