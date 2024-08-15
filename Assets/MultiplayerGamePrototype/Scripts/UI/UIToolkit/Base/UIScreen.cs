using System;
using UnityEngine.UIElements;


namespace MultiplayerGamePrototype.UI.UIToolkit.Base
{
    /// <summary>
    /// Base class for managing UI Toolkit-based screens Derive classes to manage the main parts of the UI
    ///
    /// View includes to methods to:
    ///     - Initialize the button click events and document settings
    ///     - Hide and show the parent UI element
    /// </summary>
    public abstract class UIScreen
    {
        private const string _visibleClass = "screen-visible";
        private const string _hiddenClass = "screen-hidden";

        protected VisualElement _rootElement;
        protected bool _isTransparent;
        public bool IsTransparent => _isTransparent;


        // Constructor
        public UIScreen(VisualElement rootElement)
        {
            // Required topmost VisualElement 
            _rootElement = rootElement ?? throw new ArgumentNullException(nameof(rootElement));
            Initialize();
        }

        protected virtual void Initialize() {}

        protected abstract void SetVisualElements();
        protected abstract void RegisterVisualElementsCallbacks();
        protected abstract void UnregisterVisualElementsCallbacks();


        public virtual void Show()
        {
            _rootElement.style.display = DisplayStyle.Flex;
        }

        public virtual void Hide()
        {
            _rootElement.style.display = DisplayStyle.None;
        }

        // Unregister events etc
        public virtual void Disable() 
        {
            UnregisterVisualElementsCallbacks();
        }
    }
}