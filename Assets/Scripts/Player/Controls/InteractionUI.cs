using R3;
using Scripts.Interaction;
using Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CameraType = Scripts.Managers.CameraType;

namespace Scripts.Player.Controls
{
    public class InteractionUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject promptContainer;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private Image promptIcon;
        
        [Header("Positioning")]
        [SerializeField] private float offsetDistance = 0.4f;
        [SerializeField] private float heightOffset = 0.5f;
        [SerializeField] private bool faceCamera = true;
        
        private Camera _currentCamera;
        private Interactable _currentInteractable;
        private PlayerInteraction _playerInteraction;
        private SerialDisposable _cameraSubscription = new SerialDisposable();

        private void Awake()
        {
            if (promptContainer != null)
                promptContainer.SetActive(false);
                
            // Subscribe to camera manager
            if (CameraManager.HasInstance)
            {
                CameraManager.Instance.CurrentCamera
                    .Where(cameraDetails => cameraDetails?.Type == CameraType.Player)
                    .Subscribe(cameraDetails =>
                    {
                        _currentCamera = cameraDetails?.Camera;
                        SetupCameraPositionUpdates();
                    })
                    .AddTo(this);
            }
            
            // Subscribe to player manager
            if (PlayerManager.HasInstance)
            {
                PlayerManager.Instance.PlayerInteraction
                    .Subscribe(playerInteraction =>
                    {
                        _playerInteraction = playerInteraction;
                        SetupPlayerInteractionSubscription();
                    })
                    .AddTo(this);
            }
            else
            {
                Debug.LogError("PlayerManager not found! Make sure PlayerManager is in the scene.");
            }
        }
        
        private void SetupPlayerInteractionSubscription()
        {
            if (_playerInteraction == null) return;
            
            // Subscribe to closest interactable changes
            _playerInteraction.ClosestInteractable
                .Subscribe(interactable =>
                {
                    _currentInteractable = interactable;
                    UpdatePromptVisibility(interactable != null);
                })
                .AddTo(this);
        }
        
        private void SetupCameraPositionUpdates()
        {
            if (_currentCamera == null) return;
            
            // Dispose previous subscription
            _cameraSubscription.Disposable?.Dispose();
            
            // Create new subscription for camera-based positioning
            _cameraSubscription.Disposable = Observable.EveryUpdate()
                .Where(_ => _currentInteractable != null && promptContainer != null && promptContainer.activeSelf)
                .Subscribe(_ => UpdatePromptPosition())
                .AddTo(this);
        }

        private void UpdatePromptVisibility(bool visible)
        {
            if (promptContainer != null)
            {
                promptContainer.SetActive(visible);
            }
        }

        private void UpdatePromptPosition()
        {
            if (_currentInteractable == null || _currentCamera == null || promptContainer == null) return;
            
            // Get direction from interactable to camera
            Vector3 toCamera = (_currentCamera.transform.position - _currentInteractable.Position.position).normalized;
            
            // Position the prompt slightly toward the camera
            Vector3 promptPosition = _currentInteractable.Position.position + (toCamera * offsetDistance);
            promptPosition.y += heightOffset;
            
            // Set world position on the prompt container instead of this transform
            promptContainer.transform.position = promptPosition;
            
            // Make it face the camera if enabled
            if (faceCamera && _currentCamera != null)
            {
                promptContainer.transform.LookAt(promptContainer.transform.position + _currentCamera.transform.rotation * Vector3.forward,
                    _currentCamera.transform.rotation * Vector3.up);
            }
        }
        
        public void SetPromptText(string text)
        {
            if (promptText != null)
                promptText.text = text;
        }
        
        public void SetPromptIcon(Sprite icon)
        {
            if (promptIcon != null && icon != null)
            {
                promptIcon.sprite = icon;
                promptIcon.enabled = true;
            }
            else if (promptIcon != null)
            {
                promptIcon.enabled = false;
            }
        }
        
        private void OnDestroy()
        {
            _cameraSubscription?.Dispose();
        }
    }
}