using System.Collections;
using UnityEngine;

namespace Player.Components
{
    public class ReactionComponent : IControllerComponent
    {
        private Camera _camera;
        private GameObject _body;

        private Vector3 _camStartPos;
        private Vector3 _targetCamPos;
        private bool isInAnimation;

        public ReactionComponent(Camera camera, GameObject body)
        {
            _camera = camera;
            _body = body;

            _camStartPos = camera.transform.position;
        }
        
        public void Update()
        {
            var _interpolationSpeed = 1.5f;
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                PlayerController.Instance.StartCoroutine(StartReactionAnim());
            }

            if (isInAnimation)
            {
                _camera.gameObject.transform.position = Vector3.Lerp(
                    _camera.gameObject.transform.position, _targetCamPos,
                    Time.deltaTime * _interpolationSpeed);
            }
        }

        private IEnumerator StartReactionAnim()
        {
            PlayerController.Instance.SetInputLock(true);
            
            var scale = 20f;
            isInAnimation = true;

            _targetCamPos = _camStartPos + (_camera.transform.up * scale) + (-_camera.transform.forward * scale);
            
            yield return new WaitForSecondsRealtime(1.5f);
            yield return new WaitForSecondsRealtime(1.0f);
            
            _targetCamPos = _camStartPos;
            
            yield return new WaitForSecondsRealtime(1.0f);
            
            PlayerController.Instance.SetInputLock(false);

            isInAnimation = false;
        }
    }
}