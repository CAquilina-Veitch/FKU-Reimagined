using R3;
using Scripts.Behaviours;
using Scripts.Interaction;
using UnityEngine;

namespace Scripts.Managers
{
    public class PlayerManager : SingletonBehaviour<PlayerManager>
    {
        private readonly ReactiveProperty<PlayerInteraction> _playerInteraction = new ReactiveProperty<PlayerInteraction>(null);
        private readonly ReactiveProperty<Transform> _playerTransform = new ReactiveProperty<Transform>(null);
        private readonly ReactiveProperty<GameObject> _playerGameObject = new ReactiveProperty<GameObject>(null);
        
        public ReadOnlyReactiveProperty<PlayerInteraction> PlayerInteraction => _playerInteraction;
        public ReadOnlyReactiveProperty<Transform> PlayerTransform => _playerTransform;
        public ReadOnlyReactiveProperty<GameObject> PlayerGameObject => _playerGameObject;
        
        protected override void OnAwake()
        {
            // Initialize with null values
            _playerInteraction.Value = null;
            _playerTransform.Value = null;
            _playerGameObject.Value = null;
        }
        
        public void RegisterPlayer(PlayerInteraction playerInteraction)
        {
            if (playerInteraction == null)
            {
                Debug.LogWarning("Attempted to register null PlayerInteraction");
                return;
            }
            
            if (_playerInteraction.Value != null && _playerInteraction.Value != playerInteraction)
            {
                Debug.LogWarning($"PlayerInteraction is already registered. Overwriting {_playerInteraction.Value.name} with {playerInteraction.name}");
            }
            
            _playerInteraction.Value = playerInteraction;
            _playerTransform.Value = playerInteraction.transform;
            _playerGameObject.Value = playerInteraction.gameObject;
            
            Debug.Log($"Registered player: {playerInteraction.name}");
        }
        
        public void UnregisterPlayer(PlayerInteraction playerInteraction)
        {
            if (_playerInteraction.Value == playerInteraction)
            {
                _playerInteraction.Value = null;
                _playerTransform.Value = null;
                _playerGameObject.Value = null;
                
                Debug.Log($"Unregistered player: {playerInteraction.name}");
            }
        }
        
        public bool HasPlayer()
        {
            return _playerInteraction.Value != null;
        }
        
        protected override void OnBeforeDestroy()
        {
            _playerInteraction?.Dispose();
            _playerTransform?.Dispose();
            _playerGameObject?.Dispose();
        }
    }
}