using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputController : MonoBehaviour
{
    [SerializeField] private InputAction moveInput;
    [SerializeField] private InputAction jumpButton;
    [SerializeField] private InputAction observeInput;
    
    [SerializeField] private float jumpBufferTime = 0.1f;
    private float jumpBufferTimer = 0f;

    private void Start()
    {
        moveInput.Enable();
        jumpButton.Enable();
        observeInput.Enable();
    }

    public void CheckMoveInput(ref float moveInputValue, ref bool jumpButtonBool)
    {
        moveInputValue = moveInput.ReadValue<float>();
        if (!jumpButtonBool)
            jumpButtonBool = jumpButton.triggered;
    }

    public void CheckObserveInput(ref float observeValue)
    {
        observeValue = observeInput.ReadValue<float>();
    }
    
    public void CheckObserveInput(ref bool isObserveInput)
    {
        isObserveInput = Mathf.Abs(observeInput.ReadValue<float>()) >= 0.1f;
    }

    public void ResetButton(ref bool jumpButtonBool)
    {
        jumpButtonBool = false;
    }
}
