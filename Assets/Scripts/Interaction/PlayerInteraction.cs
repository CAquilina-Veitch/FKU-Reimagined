using R3;
using Scripts.Managers;
using Scripts.Player.Controls;
using UnityEngine;

namespace Scripts.Interaction
{
    public class PlayerInteraction : Interactor
    {
        protected override void Start()
        {
            base.Start();
            
            // Register with PlayerManager
            if (PlayerManager.HasInstance)
            {
                PlayerManager.Instance.RegisterPlayer(this);
            }
            else
            {
                Debug.LogError("PlayerManager not found! Make sure PlayerManager is in the scene.");
            }
            
            // Subscribe to input system
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnInteractPressed
                    .Subscribe(_ => TryInteract())
                    .AddTo(this);
            }
            else
            {
                Debug.LogError("InputManager not found! Make sure InputManager is in the scene.");
            }
        }

        protected override void OnClosestInteractableChanged(Interactable previous, Interactable current)
        {
            base.OnClosestInteractableChanged(previous, current);
            
            // Could add sound effects or other feedback here
            if (current != null)
            {
                Debug.Log($"New interactable in range: {current.name}");
            }
        }

        protected override void OnInteractionPerformed(Interactable interactable)
        {
            base.OnInteractionPerformed(interactable);
            
            // Could add interaction feedback like sounds or particles
            Debug.Log($"Player interacted with: {interactable.name}");
        }
        
        protected override void OnDestroy()
        {
            // Unregister from PlayerManager
            if (PlayerManager.HasInstance)
            {
                PlayerManager.Instance.UnregisterPlayer(this);
            }
            
            base.OnDestroy();
        }
    }
}