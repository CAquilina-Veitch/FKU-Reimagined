using System;
using System.Collections.Generic;
using R3;
using Scripts.Behaviours;
using UnityEngine;

namespace Scripts.Managers
{
    public enum CameraType
    {
        None,
        Player,
        Map,
        Cinematic
    }
    
    [Serializable]
    public class CameraDetails
    {
        public CameraType Type { get; }
        public Camera Camera { get; }
        
        public CameraDetails(CameraType type, Camera camera)
        {
            Type = type;
            Camera = camera;
        }
    }
    
    public class CameraManager : SingletonBehaviour<CameraManager>
    {
        private readonly Dictionary<CameraType, Camera> _cameras = new Dictionary<CameraType, Camera>();
        private readonly ReactiveProperty<CameraDetails> _currentCamera = new ReactiveProperty<CameraDetails>(null);
        
        public ReadOnlyReactiveProperty<CameraDetails> CurrentCamera => _currentCamera;
        
        protected override void OnAwake()
        {
            // Initialize with None type
            _currentCamera.Value = new CameraDetails(CameraType.None, null);
        }
        
        public void RegisterCamera(CameraType type, Camera camera)
        {
            if (camera == null)
            {
                Debug.LogWarning($"Attempted to register null camera for type {type}");
                return;
            }
            
            if (_cameras.ContainsKey(type))
            {
                Debug.LogWarning($"Camera type {type} is already registered. Overwriting.");
            }
            
            _cameras[type] = camera;
            Debug.Log($"Registered camera {camera.name} as {type}");
        }
        
        public void UnregisterCamera(CameraType type)
        {
            if (_cameras.ContainsKey(type))
            {
                // If we're unregistering the current camera, switch to None
                if (_currentCamera.Value?.Type == type)
                {
                    SetCurrentCamera(CameraType.None);
                }
                
                _cameras.Remove(type);
                Debug.Log($"Unregistered camera type {type}");
            }
        }
        
        public void SetCurrentCamera(CameraType type)
        {
            if (type == CameraType.None)
            {
                _currentCamera.Value = new CameraDetails(CameraType.None, null);
                Debug.Log("Set current camera to None");
                return;
            }
            
            if (_cameras.TryGetValue(type, out Camera camera))
            {
                // Disable all other cameras
                foreach (var kvp in _cameras)
                {
                    if (kvp.Value != null)
                    {
                        kvp.Value.gameObject.SetActive(kvp.Key == type);
                    }
                }
                
                _currentCamera.Value = new CameraDetails(type, camera);
                Debug.Log($"Set current camera to {type}");
            }
            else
            {
                Debug.LogWarning($"No camera registered for type {type}");
            }
        }
        
        public Camera GetCamera(CameraType type)
        {
            return _cameras.TryGetValue(type, out Camera camera) ? camera : null;
        }
        
        public bool HasCamera(CameraType type)
        {
            return _cameras.ContainsKey(type) && _cameras[type] != null;
        }
        
        protected override void OnBeforeDestroy()
        {
            _currentCamera?.Dispose();
        }
    }
}