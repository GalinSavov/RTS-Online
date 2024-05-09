using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Cameras
{
    public class CameraController : NetworkBehaviour
    {

        [SerializeField] private Transform _cinemachineCameraTransform = null;
        [SerializeField] private float _speed = 20f;
        [SerializeField] private float _screenBorderOffset = 10f;
        [SerializeField] private Vector2 _screenXLimits = Vector2.zero;
        [SerializeField] private Vector2 _screenZLimits = Vector2.zero;

        private Vector2 _previousInput;

        //input actions
        private Controls _controls;

        //this is executed when my player object spawns in the game
        public override void OnStartAuthority()
        {
            _cinemachineCameraTransform.gameObject.SetActive(true);

            _controls = new Controls();

            _controls.Player.MoveCamera.performed += SetPreviousInput; // when a binding is being pressed down
            _controls.Player.MoveCamera.canceled += SetPreviousInput; // when a binding is being released

            //enable the controls
            _controls.Enable();
        }

        [ClientCallback]
        private void Update()
        {
            if (!isOwned || !Application.isFocused) return;

            UpdateCameraPosition();
        }

        [ClientCallback]
        private void LateUpdate()
        {
        }

        /// <summary>
        /// Updates the camera's position based on keyboard input or mouse position
        /// </summary>
        private void UpdateCameraPosition()
        {
            Vector3 _cameraPosition = _cinemachineCameraTransform.position;
            //if there was no keyboard input, check if the mouse is at 1 of the corners of the screen
            if (_previousInput == Vector2.zero)
            {
                Vector3 _cursorMovement = Vector3.zero;
                Vector2 _cursorPosition = Mouse.current.position.ReadValue();

                //check if the mouse cursor is near the top of the screen
                if (_cursorPosition.y >= Screen.height - _screenBorderOffset)
                    _cursorMovement.z += 1;

                //check if the mouse cursor is near the bottom of the screen
                else if (_cursorPosition.y <= _screenBorderOffset)
                    _cursorMovement.z -= 1;

                //check if the mouse cursor is near the right of the screen
                if (_cursorPosition.x >= Screen.width - _screenBorderOffset)
                    _cursorMovement.x += 1;

                //check if the mouse cursor is near the left of the screen
                else if (_cursorPosition.x <= _screenBorderOffset)
                    _cursorMovement.x -= 1;

                //normalized to make sure if returns the same value of 1 when moving diagonally
                _cameraPosition += _cursorMovement.normalized * _speed * Time.deltaTime;
            }

            //if there was keyboard input
            else
            {
                _cameraPosition += new Vector3(_previousInput.x, 0, _previousInput.y) * _speed * Time.deltaTime;
            }
            //when the camera goes past the screen x and screen z limits, it does not go any further
            _cameraPosition.x = Mathf.Clamp(_cameraPosition.x, _screenXLimits.x, _screenXLimits.y);
            _cameraPosition.z = Mathf.Clamp(_cameraPosition.z, _screenZLimits.x, _screenZLimits.y);

            //move the camera
            _cinemachineCameraTransform.position = _cameraPosition;
        }

        /// <summary>
        /// Update the input value when a binding is held down or released
        /// </summary>
        /// <param name="ctx"></param>
        private void SetPreviousInput(InputAction.CallbackContext ctx)
        {
            _previousInput = ctx.ReadValue<Vector2>();
        }
    }

}
