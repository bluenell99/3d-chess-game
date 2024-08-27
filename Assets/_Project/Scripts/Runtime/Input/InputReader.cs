using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInput;

[CreateAssetMenu(menuName = "Chess/Input/Input Reader", fileName = "New Input Reader")]
public class InputReader : ScriptableObject, IPlayerActions
{

    private PlayerInput _input;


    public Vector2 MousePointer => _input.Player.Point.ReadValue<Vector2>();
    
    public event Action onSelectPressed;

    private void OnEnable()
    {
        if (_input == null)
        {
            _input = new PlayerInput();
            _input.Player.SetCallbacks(this);
        }
    }
    
    public void Enable()
    {
        _input.Enable();
    }
    
    public void OnMove(InputAction.CallbackContext context) { }

    public void OnLook(InputAction.CallbackContext context) { }

    public void OnFire(InputAction.CallbackContext context) { }

 
    public void OnSelect(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        onSelectPressed?.Invoke();;
    }

    public void OnPoint(InputAction.CallbackContext context) { }
}
