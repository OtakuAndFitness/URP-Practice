using System;
using SRDebugger.Internal;
using SRDebugger.UI;
using SRF;
using SRF.Service;
using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.Services.Implementation
{
    [Service(typeof(IDebugPanelService))]
    public class DebugPanelServiceImpl : ScriptableObject, IDebugPanelService
    {
        private bool? _cursorWasVisible;
        private DebugPanelRoot _debugPanelRootObject;
        private bool _isVisible;
        public event Action<IDebugPanelService, bool> VisibilityChanged;

#region SRModify_BlockRegularTouchEvents
        private GameObject nguiCamera;
#endregion

        public bool IsLoaded
        {
            get { return _debugPanelRootObject != null; }
        }

        public bool IsVisible
        {
            get { return IsLoaded && _isVisible; }
            set
            {
                if (_isVisible == value)
                {
                    return;
                }

                if (value)
                {
                    if (!IsLoaded)
                    {
                        Load();
                    }

#region SRModify_BlockRegularTouchEvents
                    var uiRoot = GameObject.Find("UI Root");
                    if(uiRoot)
                    {
                        var t = uiRoot.transform.Find("Camera");
                        if(t) nguiCamera = t.gameObject;
                    }
#endregion

                    SRDebuggerUtil.EnsureEventSystemExists();

                    _debugPanelRootObject.CanvasGroup.alpha = 1.0f;
                    _debugPanelRootObject.CanvasGroup.interactable = true;
                    _debugPanelRootObject.CanvasGroup.blocksRaycasts = true;
					GraphicRaycaster[] casters = _debugPanelRootObject.GetComponentsInChildren<GraphicRaycaster> (true);
					foreach (GraphicRaycaster caster in casters) {
						caster.enabled = true;
					}

#if UNITY_5
                    _cursorWasVisible = Cursor.visible;
#else
					_cursorWasVisible = Cursor.visible;
#endif
                }
                else
                {
                    if (IsLoaded)
                    {
                        _debugPanelRootObject.CanvasGroup.alpha = 0.0f;
                        _debugPanelRootObject.CanvasGroup.interactable = false;
                        _debugPanelRootObject.CanvasGroup.blocksRaycasts = false;
						GraphicRaycaster[] casters = _debugPanelRootObject.GetComponentsInChildren<GraphicRaycaster> (true);
						foreach (GraphicRaycaster caster in casters) {
							caster.enabled = false;
						}
                    }

                    if (_cursorWasVisible.HasValue)
                    {
#if UNITY_5
                        Cursor.visible = _cursorWasVisible.Value;
#else
						Cursor.visible = _cursorWasVisible.Value;
#endif
                        _cursorWasVisible = null;
                    }
                }

                _isVisible = value;
#region SRModify_BlockRegularTouchEvents
                if(nguiCamera) nguiCamera.SetActive(!value);
#endregion

                if (VisibilityChanged != null)
                {
                    VisibilityChanged(this, _isVisible);
                }
            }
        }

        public DefaultTabs? ActiveTab
        {
            get
            {
                if (_debugPanelRootObject == null)
                {
                    return null;
                }

                return _debugPanelRootObject.TabController.ActiveTab;
            }
        }

        public void OpenTab(DefaultTabs tab)
        {
            if (!IsVisible)
            {
                IsVisible = true;
            }

            _debugPanelRootObject.TabController.OpenTab(tab);
        }

        public void Unload()
        {
            if (_debugPanelRootObject == null)
            {
                return;
            }

            IsVisible = false;

            _debugPanelRootObject.CachedGameObject.SetActive(false);
            Destroy(_debugPanelRootObject.CachedGameObject);

            _debugPanelRootObject = null;
        }

        private void Load()
        {
            var prefab = Resources.Load<DebugPanelRoot>(SRDebugPaths.DebugPanelPrefabPath);

            if (prefab == null)
            {
                Debug.LogError("[SRDebugger] Error loading debug panel prefab");
                return;
            }

            _debugPanelRootObject = SRInstantiate.Instantiate(prefab);
            _debugPanelRootObject.name = "Panel";

            DontDestroyOnLoad(_debugPanelRootObject);

            _debugPanelRootObject.CachedTransform.SetParent(Hierarchy.Get("SRDebugger"), true);

            SRDebuggerUtil.EnsureEventSystemExists();
        }
    }
}
