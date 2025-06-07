using R3;
using UnityEngine;

namespace Scripts.Interaction
{
    public enum InteractionState
    {
        None,       // Not available for interaction
        Ready,      // Available and ready for interaction
        Busy,       // Currently processing an interaction
        Unfulfilled // Requirements not met for interaction
    }

    public class Interactable : MonoBehaviour
    {
        [SerializeField] protected InteractionState initialState = InteractionState.Ready;
        [SerializeField] protected string interactionPrompt = "Interact";
        
        public Transform Position => interactablePosition;
        private Transform interactablePosition;
        public ReadOnlyReactiveProperty<InteractionState> State => state;
        protected readonly ReactiveProperty<InteractionState> state = new();
        
        public string InteractionPrompt => interactionPrompt;

        [SerializeField] private Collider collider;

        protected virtual void Awake()
        {
            if(collider == null) Debug.LogWarning($"{gameObject} interactable has no collider,");
            state.Value = initialState;
            interactablePosition = transform;
        }

        public virtual bool CanInteract()
        {
            return state.Value == InteractionState.Ready;
        }

        public virtual void Interact(Interactor interactor = null)
        {
            if (!CanInteract())
            {
                Debug.LogWarning($"Cannot interact with {gameObject.name}. Current state: {state.Value}");
                return;
            }

            Debug.Log($"Interacted with {gameObject.name}!");
            OnInteract(interactor);
        }

        protected virtual void OnInteract(Interactor interactor)
        {
            // Override in derived classes for specific interaction behavior
        }

        public void SetState(InteractionState newState) => state.Value = newState;
        
    }
}