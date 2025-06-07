using System.Collections.Generic;
using UnityEngine;
using Scripts.Behaviours;
using Scripts.UI;
using R3;

namespace Scripts.Managers
{
    public class UIManager : SingletonBehaviour<UIManager>
    {
        [Header("UI Settings")]
        [SerializeField] private bool allowMultipleWindows = false;
        [SerializeField] private bool pauseTimeWhenUIOpen = true;
        
        private HashSet<UIWindow> activeWindows = new HashSet<UIWindow>();
        private readonly Subject<HashSet<UIWindow>> activeWindowsSubject = new Subject<HashSet<UIWindow>>();
        
        public ReadOnlyReactiveProperty<HashSet<UIWindow>> ActiveWindows { get; private set; }
        public bool IsAnyWindowOpen => activeWindows.Count > 0;
        
        protected override void OnAwake()
        {
            ActiveWindows = activeWindowsSubject.ToReadOnlyReactiveProperty(new HashSet<UIWindow>());
        }
        
        public void OpenWindow(UIWindow window)
        {
            if (window == UIWindow.None) return;
            
            if (!allowMultipleWindows && activeWindows.Count > 0)
            {
                CloseAllWindows();
            }
            
            if (activeWindows.Add(window))
            {
                Debug.Log($"Opened UI Window: {window}");
                OnWindowOpened(window);
                NotifyWindowsChanged();
            }
        }
        
        public void CloseWindow(UIWindow window)
        {
            if (window == UIWindow.None) return;
            
            if (activeWindows.Remove(window))
            {
                Debug.Log($"Closed UI Window: {window}");
                OnWindowClosed(window);
                NotifyWindowsChanged();
            }
        }
        
        public void ToggleWindow(UIWindow window)
        {
            if (IsWindowOpen(window))
            {
                CloseWindow(window);
            }
            else
            {
                OpenWindow(window);
            }
        }
        
        public void CloseAllWindows()
        {
            var windowsToClose = new List<UIWindow>(activeWindows);
            foreach (var window in windowsToClose)
            {
                CloseWindow(window);
            }
        }
        
        public bool IsWindowOpen(UIWindow window)
        {
            return activeWindows.Contains(window);
        }
        
        private void NotifyWindowsChanged()
        {
            activeWindowsSubject.OnNext(new HashSet<UIWindow>(activeWindows));
            UpdateGameState();
        }
        
        private void OnWindowOpened(UIWindow window)
        {
            // Handle cursor state
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // Specific window opening logic
            switch (window)
            {
                case UIWindow.Dialogue:
                    // Don't pause for dialogue
                    break;
                default:
                    if (pauseTimeWhenUIOpen)
                    {
                        Time.timeScale = 0f;
                    }
                    break;
            }
        }
        
        private void OnWindowClosed(UIWindow window)
        {
            // Check if any windows are still open
            if (!IsAnyWindowOpen)
            {
                // Restore cursor state
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
                // Restore time scale
                if (pauseTimeWhenUIOpen)
                {
                    Time.timeScale = 1f;
                }
            }
        }
        
        private void UpdateGameState()
        {
            // You can add additional game state management here
            // For example, disabling player movement when UI is open
        }
        
        protected override void OnBeforeDestroy()
        {
            activeWindowsSubject?.Dispose();
        }
    }
}