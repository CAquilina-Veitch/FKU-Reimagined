using UnityEngine;

namespace Scripts.Interaction
{
    public class Interactable : MonoBehaviour
    {
        public Transform Position => transform;

        public void Interact()
        {
            Debug.Log("Interacted!");
        }
    }
}