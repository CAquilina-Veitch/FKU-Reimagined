using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using R3.Triggers;
using UnityEngine;

namespace Scripts.Interaction
{
    public abstract class Interactor : MonoBehaviour
    {
        public Transform Position => transform;
        [SerializeField] protected Collider interactionCollider;
        [SerializeField] protected List<Interactable> interactables = new List<Interactable>();
        
        // Track the current closest interactable for UI purposes
        protected readonly ReactiveProperty<Interactable> _closestInteractable = new ReactiveProperty<Interactable>(null);
        public ReadOnlyReactiveProperty<Interactable> ClosestInteractable => _closestInteractable;

        protected virtual void Awake()
        {
            if (interactionCollider == null)
            {
                Debug.LogError($"Interaction collider not set on {gameObject.name}");
                return;
            }
            
            SetupTriggerObservables();
        }

        protected virtual void Start()
        {
            // Update closest interactable whenever the list changes
            Observable.EveryUpdate()
                .Where(_ => interactables.Count > 0)
                .Subscribe(_ => UpdateClosestInteractable())
                .AddTo(this);
        }

        private void SetupTriggerObservables()
        {
            interactionCollider.OnTriggerEnterAsObservable()
                .Subscribe(other =>
                {
                    Debug.Log($"Trigger Enter: {other.name}");
                    if (other.TryGetComponent<Interactable>(out var interactable))
                    {
                        Debug.Log($"Found Interactable component on {other.name}");
                        AddInteractable(interactable);
                    }
                })
                .AddTo(this);
            
            interactionCollider.OnTriggerExitAsObservable()
                .Subscribe(other =>
                {
                    Debug.Log($"Trigger Exit: {other.name}");
                    if (other.TryGetComponent<Interactable>(out var interactable))
                    {
                        Debug.Log($"Found Interactable component on {other.name} for removal");
                        RemoveInteractable(interactable);
                    }
                })
                .AddTo(this);
        }

        protected virtual void AddInteractable(Interactable newInteractable)
        {
            if (newInteractable == null) return;
            
            // Check if already in list to avoid duplicates
            if (!interactables.Contains(newInteractable))
            {
                interactables.Add(newInteractable);
                Debug.Log($"Added interactable: {newInteractable.name}. Total: {interactables.Count}");
                UpdateClosestInteractable();
                OnInteractableAdded(newInteractable);
            }
        }

        protected virtual void RemoveInteractable(Interactable removeInteractable)
        {
            if (removeInteractable == null) return;
            
            bool wasRemoved = interactables.Remove(removeInteractable);
            if (wasRemoved)
            {
                Debug.Log($"Removed interactable: {removeInteractable.name}. Total: {interactables.Count}");
                
                // If we removed the closest one, update
                if (_closestInteractable.Value == removeInteractable)
                {
                    UpdateClosestInteractable();
                }
                OnInteractableRemoved(removeInteractable);
            }
            else
            {
                Debug.LogWarning($"Tried to remove interactable {removeInteractable.name} but it wasn't in the list");
            }
        }

        protected virtual void UpdateClosestInteractable()
        {
            if (interactables.Count == 0)
            {
                _closestInteractable.Value = null;
                return;
            }

            var previousClosest = _closestInteractable.Value;
            _closestInteractable.Value = interactables
                .OrderBy(i => Vector3.Distance(i.Position.position, Position.position))
                .FirstOrDefault();
                
            if (previousClosest != _closestInteractable.Value)
            {
                OnClosestInteractableChanged(previousClosest, _closestInteractable.Value);
            }
        }

        public virtual void TryInteract()
        {
            if (_closestInteractable.Value != null)
            {
                _closestInteractable.Value.Interact();
                OnInteractionPerformed(_closestInteractable.Value);
            }
            else
            {
                Debug.LogWarning($"{gameObject} tried to interact, but no interactables were found.");
            }
        }
        
        // Virtual hooks for derived classes
        protected virtual void OnInteractableAdded(Interactable interactable) { }
        protected virtual void OnInteractableRemoved(Interactable interactable) { }
        protected virtual void OnClosestInteractableChanged(Interactable previous, Interactable current) { }
        protected virtual void OnInteractionPerformed(Interactable interactable) { }
        
        protected virtual void OnDestroy()
        {
            _closestInteractable?.Dispose();
        }
    }
}