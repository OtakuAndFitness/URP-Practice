using System.Collections.Generic;
using SRDebugger.Internal;
using SRDebugger.UI.Controls;
using SRDebugger.UI.Other;
using SRF;
using SRF.Service;
using UnityEngine;

namespace SRDebugger.Services.Implementation
{
    [Service(typeof (IPinnedUIService))]
    public class PinnedUIServiceImpl : SRServiceBase<IPinnedUIService>, IPinnedUIService
    {
        private readonly List<OptionsControlBase> _controlList = new List<OptionsControlBase>();

        private readonly Dictionary<OptionDefinition, OptionsControlBase> _pinnedObjects =
            new Dictionary<OptionDefinition, OptionsControlBase>();

        private bool _queueRefresh;
        private PinnedUIRoot _uiRoot;

        public DockConsoleController DockConsoleController
        {
            get
            {
                if (_uiRoot == null)
                {
                    Load();
                }
                return _uiRoot.DockConsoleController;
            }
        }

        public bool IsProfilerPinned
        {
            get
            {
                if (_uiRoot == null)
                {
                    return false;
                }
                return _uiRoot.Profiler.activeSelf;
            }
            set
            {
                if (_uiRoot == null)
                {
                    Load();
                }
                _uiRoot.Profiler.SetActive(value);
            }
        }

        public void Pin(OptionDefinition obj, int order = -1)
        {
            if (_uiRoot == null)
            {
                Load();
            }

            var control = OptionControlFactory.CreateControl(obj);

            control.CachedTransform.SetParent(_uiRoot.Container, false);

            if (order >= 0)
            {
                control.CachedTransform.SetSiblingIndex(order);
            }

            _pinnedObjects.Add(obj, control);
            _controlList.Add(control);
        }

        public void Unpin(OptionDefinition obj)
        {
            if (!_pinnedObjects.ContainsKey(obj))
            {
                Debug.LogWarning("[SRDebugger.PinnedUI] Attempted to unpin option which isn't pinned.");
                return;
            }

            var control = _pinnedObjects[obj];

            _pinnedObjects.Remove(obj);
            _controlList.Remove(control);

            Destroy(control.CachedGameObject);
        }

        public bool HasPinned(OptionDefinition option)
        {
            return _pinnedObjects.ContainsKey(option);
        }

        protected override void Awake()
        {
            base.Awake();

            CachedTransform.SetParent(Hierarchy.Get("SRDebugger"));
        }

        private void Load()
        {
            var prefab = Resources.Load<PinnedUIRoot>(SRDebugPaths.PinnedUIPrefabPath);

            if (prefab == null)
            {
                Debug.LogError("[SRDebugger.PinnedUI] Error loading ui prefab");
                return;
            }

            var instance = SRInstantiate.Instantiate(prefab);
            instance.CachedTransform.SetParent(CachedTransform, false);

            _uiRoot = instance;
            UpdateAnchors();
            SRDebug.Instance.PanelVisibilityChanged += OnDebugPanelVisibilityChanged;

            SROptions.Current.PropertyChanged += OptionsOnPropertyChanged;
        }

        private void UpdateAnchors()
        {
            // Setup alignment of Profiler/Options splitter
            switch (Settings.Instance.ProfilerAlignment)
            {
                case PinAlignment.BottomLeft:
                case PinAlignment.TopLeft:
                    _uiRoot.Profiler.transform.SetSiblingIndex(0);
                    break;

                case PinAlignment.BottomRight:
                case PinAlignment.TopRight:
                    _uiRoot.Profiler.transform.SetSiblingIndex(1);
                    break;
            }

            // Setup alignment of Profiler vertical layout group
            switch (Settings.Instance.ProfilerAlignment)
            {
                case PinAlignment.TopRight:
                case PinAlignment.TopLeft:
                    _uiRoot.ProfilerVerticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
                    break;

                case PinAlignment.BottomRight:
                case PinAlignment.BottomLeft:
                    _uiRoot.ProfilerVerticalLayoutGroup.childAlignment = TextAnchor.LowerCenter;
                    break;
            }

            _uiRoot.ProfilerHandleManager.SetAlignment(Settings.Instance.ProfilerAlignment);

            // Setup alignment of options flow layout group
            switch (Settings.Instance.OptionsAlignment)
            {
                case PinAlignment.BottomLeft: // OptionsBottomLeft
                    _uiRoot.OptionsLayoutGroup.childAlignment = TextAnchor.LowerLeft;
                    break;

                case PinAlignment.TopLeft:
                    _uiRoot.OptionsLayoutGroup.childAlignment = TextAnchor.UpperLeft;
                    break;

                case PinAlignment.BottomRight:
                    _uiRoot.OptionsLayoutGroup.childAlignment = TextAnchor.LowerRight;
                    break;

                case PinAlignment.TopRight:
                    _uiRoot.OptionsLayoutGroup.childAlignment = TextAnchor.UpperRight;
                    break;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (_queueRefresh)
            {
                _queueRefresh = false;
                Refresh();
            }
        }

        private void OptionsOnPropertyChanged(object sender, string propertyName)
        {
            _queueRefresh = true;
        }

        private void OnDebugPanelVisibilityChanged(bool isVisible)
        {
            // Refresh bindings when debug panel is no longer visible
            if (!isVisible)
            {
                _queueRefresh = true;
            }
        }

        private void Refresh()
        {
            for (var i = 0; i < _controlList.Count; i++)
            {
                _controlList[i].Refresh();
            }
        }
    }
}
