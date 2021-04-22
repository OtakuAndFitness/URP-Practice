using SRDebugger.Internal;
using SRDebugger.UI.Other;
using SRF.Service;
using UnityEngine;

namespace SRDebugger.Services.Implementation
{
    [Service(typeof (IDockConsoleService))]
    public class DockConsoleServiceImpl : IDockConsoleService
    {
        private ConsoleAlignment _alignment;
        private DockConsoleController _consoleRoot;
        private bool _didSuspendTrigger;
        private bool _isExpanded = true;
        private bool _isVisible;

        public DockConsoleServiceImpl()
        {
            _alignment = Settings.Instance.ConsoleAlignment;
        }

        public bool IsVisible
        {
            get { return _isVisible; }

            set
            {
                if (value == _isVisible)
                {
                    return;
                }

                _isVisible = value;

                if (_consoleRoot == null && value)
                {
                    Load();
                }
                else
                {
                    _consoleRoot.CachedGameObject.SetActive(value);
                }

                CheckTrigger();
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }

            set
            {
                if (value == _isExpanded)
                {
                    return;
                }

                _isExpanded = value;

                if (_consoleRoot == null && value)
                {
                    Load();
                }
                else
                {
                    _consoleRoot.SetDropdownVisibility(value);
                }

                CheckTrigger();
            }
        }

        public ConsoleAlignment Alignment
        {
            get { return _alignment; }
            set
            {
                _alignment = value;

                if (_consoleRoot != null)
                {
                    _consoleRoot.SetAlignmentMode(value);
                }

                CheckTrigger();
            }
        }

        private void Load()
        {
            var dockService = SRServiceManager.GetService<IPinnedUIService>();

            if (dockService == null)
            {
                Debug.LogError("[DockConsoleService] PinnedUIService not found");
                return;
            }

            var pinService = dockService as PinnedUIServiceImpl;

            if (pinService == null)
            {
                Debug.LogError("[DockConsoleService] Expected IPinnedUIService to be PinnedUIServiceImpl");
                return;
            }

            _consoleRoot = pinService.DockConsoleController;

            _consoleRoot.SetDropdownVisibility(_isExpanded);
            _consoleRoot.IsVisible = _isVisible;
            _consoleRoot.SetAlignmentMode(_alignment);

            CheckTrigger();
        }

        private void CheckTrigger()
        {
            var triggerAlignment = (Service.Trigger.Position == PinAlignment.TopLeft ||
                                    Service.Trigger.Position == PinAlignment.TopRight)
                ? ConsoleAlignment.Top
                : ConsoleAlignment.Bottom;

            var shouldHide = IsVisible && Alignment == triggerAlignment;

            // Show trigger if we have hidden it, and we no longer need to hide it.
            if (_didSuspendTrigger && !shouldHide)
            {
                Service.Trigger.IsEnabled = true;
                _didSuspendTrigger = false;
            }
            else if (Service.Trigger.IsEnabled && shouldHide)
            {
                Service.Trigger.IsEnabled = false;
                _didSuspendTrigger = true;
            }
        }
    }
}
