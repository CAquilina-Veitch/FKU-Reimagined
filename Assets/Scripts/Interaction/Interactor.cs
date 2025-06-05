using System;
using System.Collections.Generic;
using R3;
using R3.Triggers;
using UnityEngine;

namespace Scripts.Interaction
{
    public class Interactor : MonoBehaviour
    {
        public Transform Position => transform;
        [SerializeField] private Collider collider;
        [SerializeField] private List<Interactable> interactables;


        private void Awake()
        {
            Interactable checkInteractable = null;
            collider.OnTriggerEnterAsObservable()
                .Where(newObject => newObject
                    .TryGetComponent(out checkInteractable))
                .Subscribe(_ => 
                    AddInteractable(checkInteractable)).AddTo(this);
            
            collider.OnTriggerExitAsObservable()
                .Where(newObject => newObject
                    .TryGetComponent(out checkInteractable))
                .Subscribe(_ => 
                    RemoveInteractable(checkInteractable)).AddTo(this);
        }

        private void AddInteractable(Interactable newInteractable)
        {
            if (newInteractable == null) return;
            interactables.Add(newInteractable);

        }

        private void RemoveInteractable(Interactable removeInteractable)
        {
            if (removeInteractable == null) return;
            interactables.Remove(removeInteractable);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryInteract();
            }
        }

        public void TryInteract()
        {
            if (interactables.Count > 0)
            {
                (Interactable, float) currentMatch = (null, Mathf.Infinity);
                foreach (var i in interactables)
                {
                    var distance = Vector3.Distance(i.Position.position, Position.position);
                    if (distance < currentMatch.Item2) 
                        currentMatch = (i, distance);
                }
                if (currentMatch.Item1 != null)
                {
                    currentMatch.Item1.Interact();
                }
            }
            else
                Debug.LogWarning($"{gameObject} tried to interact, but no interactables were found.");
        }
    }
}