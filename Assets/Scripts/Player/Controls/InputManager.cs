using System;
using R3;
using Scripts.Behaviours;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Player.Controls
{
    public class InputManager : SingletonBehaviour<InputManager>
    {
        
        // Reference to the Input Action Asset
        [SerializeField] private InputActionAsset inputActions;
        private InputActionMap playerActionMap;
        
        // Different input types for different behaviors
        
        // 1. Press events (fire once on press)
        private readonly Subject<Unit> _interactPressed = new Subject<Unit>();
        private readonly Subject<Unit> _jumpPressed = new Subject<Unit>();
        
        // 2. Hold states (true while held, false when released)
        private readonly ReactiveProperty<bool> _sprintHeld = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<bool> _aimHeld = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<bool> _crouchHeld = new ReactiveProperty<bool>(false);
        
        // 3. Toggle states (press to toggle on/off)
        private readonly ReactiveProperty<bool> _walkToggled = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<bool> _inventoryOpen = new ReactiveProperty<bool>(false);
        
        // 4. Continuous values (like movement)
        private readonly ReactiveProperty<Vector2> _moveInput = new ReactiveProperty<Vector2>(Vector2.zero);
        private readonly ReactiveProperty<Vector2> _lookInput = new ReactiveProperty<Vector2>(Vector2.zero);
        
        // 5. Press and Release events (for abilities that might charge/release)
        private readonly Subject<Unit> _ability1Pressed = new Subject<Unit>();
        private readonly Subject<Unit> _ability1Released = new Subject<Unit>();
        
        // Public observables
        public Observable<Unit> OnInteractPressed => _interactPressed;
        public Observable<Unit> OnJumpPressed => _jumpPressed;
        
        public ReadOnlyReactiveProperty<bool> IsSprintHeld => _sprintHeld;
        public ReadOnlyReactiveProperty<bool> IsAimHeld => _aimHeld;
        public ReadOnlyReactiveProperty<bool> IsCrouchHeld => _crouchHeld;
        
        public ReadOnlyReactiveProperty<bool> IsWalkToggled => _walkToggled;
        public ReadOnlyReactiveProperty<bool> IsInventoryOpen => _inventoryOpen;
        
        public ReadOnlyReactiveProperty<Vector2> MoveInput => _moveInput;
        public ReadOnlyReactiveProperty<Vector2> LookInput => _lookInput;
        
        public Observable<Unit> OnAbility1Pressed => _ability1Pressed;
        public Observable<Unit> OnAbility1Released => _ability1Released;

        protected override void OnAwake()
        {
            if (inputActions == null)
            {
                Debug.LogError("Input Actions asset not assigned to InputManager!");
                return;
            }
            
            playerActionMap = inputActions.FindActionMap("Player");
            if (playerActionMap == null)
            {
                Debug.LogError("Player action map not found in Input Actions asset!");
            }
            else
            {
                Debug.Log($"Player action map found with {playerActionMap.actions.Count} actions");
            }
        }

        private void OnEnable()
        {
            if (playerActionMap != null)
            {
                playerActionMap.Enable();
                SubscribeToInputActions();
            }
        }

        private void OnDisable()
        {
            if (playerActionMap != null)
            {
                playerActionMap.Disable();
            }
        }

        private void SubscribeToInputActions()
        {
            if (playerActionMap == null) return;
            
            // Get actions from the action map
            var interactAction = playerActionMap.FindAction("Interact");
            var jumpAction = playerActionMap.FindAction("Jump");
            var moveAction = playerActionMap.FindAction("Move");
            var lookAction = playerActionMap.FindAction("Look");
            var sprintAction = playerActionMap.FindAction("Sprint");
            var crouchAction = playerActionMap.FindAction("Crouch");
            var attackAction = playerActionMap.FindAction("Attack");
            
            // Subscribe to actions that exist
            if (interactAction != null)
            {
                // Use started for immediate response, performed for hold interactions
                interactAction.started += _ => 
                {
                    Debug.Log("Interact action started - firing interaction!");
                    _interactPressed.OnNext(Unit.Default);
                };
                interactAction.performed += _ => Debug.Log("Interact action performed!");
                interactAction.canceled += _ => Debug.Log("Interact action canceled!");
            }
            else
            {
                Debug.LogError("Interact action not found in Player action map!");
            }
            
            if (jumpAction != null)
            {
                jumpAction.performed += _ => _jumpPressed.OnNext(Unit.Default);
            }
            
            if (moveAction != null)
            {
                moveAction.performed += ctx => _moveInput.Value = ctx.ReadValue<Vector2>();
                moveAction.canceled += _ => _moveInput.Value = Vector2.zero;
            }
            
            if (lookAction != null)
            {
                lookAction.performed += ctx => _lookInput.Value = ctx.ReadValue<Vector2>();
                lookAction.canceled += _ => _lookInput.Value = Vector2.zero;
            }
            
            if (sprintAction != null)
            {
                sprintAction.performed += _ => _sprintHeld.Value = true;
                sprintAction.canceled += _ => _sprintHeld.Value = false;
            }
            
            if (crouchAction != null)
            {
                crouchAction.performed += _ => _crouchHeld.Value = true;
                crouchAction.canceled += _ => _crouchHeld.Value = false;
            }
            
            if (attackAction != null)
            {
                attackAction.performed += _ => _ability1Pressed.OnNext(Unit.Default);
                attackAction.canceled += _ => _ability1Released.OnNext(Unit.Default);
            }
        }

        protected override void OnBeforeDestroy()
        {
            _interactPressed?.Dispose();
            _jumpPressed?.Dispose();
            _sprintHeld?.Dispose();
            _aimHeld?.Dispose();
            _crouchHeld?.Dispose();
            _walkToggled?.Dispose();
            _inventoryOpen?.Dispose();
            _moveInput?.Dispose();
            _lookInput?.Dispose();
            _ability1Pressed?.Dispose();
            _ability1Released?.Dispose();
        }
    }
}