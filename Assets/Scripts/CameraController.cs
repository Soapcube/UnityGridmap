/*****************************************************************************
// File Name : CameraController.cs
// Author : Arcadia Koederitz
// Creation Date : 8/20/2026
// Last Modified : 8/20/2026
//
// Brief Description : Test script for controlling the camera at runtime.
*****************************************************************************/
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 1;
    [SerializeField] private float moveSpeed;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction elevateAction;

    private Vector2 rotation;
    private Vector3 moveInput;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (TryGetComponent(out PlayerInput pi))
        {
            moveAction = pi.currentActionMap.FindAction("Move");
            elevateAction = pi.currentActionMap.FindAction("Elevate");
            lookAction = pi.currentActionMap.FindAction("Look");

            moveAction.performed += HandleMovePerformed;
            moveAction.canceled += Handle_MoveCanceled;
            elevateAction.performed += Handle_ElevatePerformed;
            elevateAction.canceled += Handle_ElevateCanceled;
            lookAction.performed += Handle_LookPerformed;
        }
    }

    private void OnDestroy()
    {
        moveAction.performed -= HandleMovePerformed;
        moveAction.canceled -= Handle_MoveCanceled;
        elevateAction.performed -= Handle_ElevatePerformed;
        elevateAction.canceled -= Handle_ElevateCanceled;
        lookAction.performed -= Handle_LookPerformed;
    }

    private void HandleMovePerformed(InputAction.CallbackContext obj)
    {
        Vector2 input = obj.ReadValue<Vector2>();
        moveInput.x = input.x;
        moveInput.z = input.y;
    }

    private void Handle_MoveCanceled(InputAction.CallbackContext obj)
    {
        moveInput.x = moveInput.z = 0;
    }

    private void Handle_ElevatePerformed(InputAction.CallbackContext obj)
    {
        moveInput.y = obj.ReadValue<float>();
    }

    private void Handle_ElevateCanceled(InputAction.CallbackContext obj)
    {
        moveInput.y = 0;
    }

    private void Update()
    {
        transform.Translate(Time.deltaTime * moveSpeed * moveInput);
    }

    private void Handle_LookPerformed(InputAction.CallbackContext obj)
    {
        Vector2 mouseDelta = obj.ReadValue<Vector2>() * mouseSensitivity * Time.deltaTime;
        rotation.y -= mouseDelta.y;
        rotation.x += mouseDelta.x;

        rotation.y = Mathf.Clamp(rotation.y, -90, 90);

        Quaternion xRot = Quaternion.Euler(rotation.y, 0, 0);
        transform.localRotation = xRot * (Quaternion.Inverse(xRot) * Quaternion.Euler(0, rotation.x, 0) * xRot);
    }

}
