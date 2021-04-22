using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using SRF;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SRDebugger
{
    public enum DefaultTabs
    {
        SystemInformation = 0,
        Options = 1,
        Console = 2,
        Profiler = 3,
        BugReporter = 4
    }

    public enum PinAlignment
    {
        TopLeft = 0,
        TopRight = 1,
        BottomLeft = 2,
        BottomRight = 3
    }

    public enum ConsoleAlignment
    {
        Top,
        Bottom
    }

    public class Settings : ScriptableObject
    {
        public enum ShortcutActions
        {
            None = 0,

            OpenSystemInfoTab = 1,
            OpenConsoleTab = 2,
            OpenOptionsTab = 3,
            OpenProfilerTab = 4,
            OpenBugReporterTab = 5,

            ClosePanel = 6,
            OpenPanel = 7,
            TogglePanel = 8,

            ShowBugReportPopover = 9,
            ToggleDockedConsole = 10,
            ToggleDockedProfiler = 11
        }

        public enum TriggerBehaviours
        {
            TripleTap,
            TapAndHold,
            DoubleTap
        }

        public enum TriggerEnableModes
        {
            Enabled,
            MobileOnly,
            Off
        }

        private const string ResourcesPath = "/usr/Resources/SRDebugger";
        private const string ResourcesName = "Settings";
        private static Settings _instance;

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GetOrCreateInstance();
                }

                if (_instance._keyboardShortcuts != null && _instance._keyboardShortcuts.Length > 0)
                {
                    _instance.UpgradeKeyboardShortcuts();
                }

                return _instance;
            }
        }

        private static KeyboardShortcut[] GetDefaultKeyboardShortcuts()
        {
            return new[]
            {
                new KeyboardShortcut
                {
                    Control = true,
                    Shift = true,
                    Key = KeyCode.F1,
                    Action = ShortcutActions.OpenSystemInfoTab
                },
                new KeyboardShortcut
                {
                    Control = true,
                    Shift = true,
                    Key = KeyCode.F2,
                    Action = ShortcutActions.OpenConsoleTab
                },
                new KeyboardShortcut
                {
                    Control = true,
                    Shift = true,
                    Key = KeyCode.F3,
                    Action = ShortcutActions.OpenOptionsTab
                },
                new KeyboardShortcut
                {
                    Control = true,
                    Shift = true,
                    Key = KeyCode.F4,
                    Action = ShortcutActions.OpenProfilerTab
                }
            };
        }

        private void UpgradeKeyboardShortcuts()
        {
            Debug.Log("[SRDebugger] Upgrading Settings format");

            var newShortcuts = new List<KeyboardShortcut>();

            for (var i = 0; i < _keyboardShortcuts.Length; i++)
            {
                var s = _keyboardShortcuts[i];

                newShortcuts.Add(new KeyboardShortcut
                {
                    Action = s.Action,
                    Key = s.Key,
                    Alt = _keyboardModifierAlt,
                    Shift = _keyboardModifierShift,
                    Control = _keyboardModifierControl
                });
            }

            _keyboardShortcuts = new KeyboardShortcut[0];
            _newKeyboardShortcuts = newShortcuts.ToArray();

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        [Serializable]
        public sealed class KeyboardShortcut
        {
            [SerializeField] public ShortcutActions Action;

            [SerializeField] public bool Alt;

            [SerializeField] public bool Control;

            [SerializeField] public KeyCode Key;

            [SerializeField] public bool Shift;
        }

        #region Settings

        public bool IsEnabled
        {
            get { return _isEnabled; }
#if UNITY_EDITOR
            set { _isEnabled = value; }
#endif
        }

        public bool AutoLoad
        {
            get { return _autoLoad; }
#if UNITY_EDITOR
            set { _autoLoad = value; }
#endif
        }

        public DefaultTabs DefaultTab
        {
            get { return _defaultTab; }
#if UNITY_EDITOR
            set { _defaultTab = value; }
#endif
        }

        /// <summary>
        /// Enable the triple-tap button.
        /// </summary>
        public TriggerEnableModes EnableTrigger
        {
            get { return _triggerEnableMode; }
#if UNITY_EDITOR
            set { _triggerEnableMode = value; }
#endif
        }

        /// <summary>
        /// Enable the triple-tap button.
        /// </summary>
        public TriggerBehaviours TriggerBehaviour
        {
            get { return _triggerBehaviour; }
#if UNITY_EDITOR
            set { _triggerBehaviour = value; }
#endif
        }

        public bool EnableKeyboardShortcuts
        {
            get { return _enableKeyboardShortcuts; }
#if UNITY_EDITOR
            set { _enableKeyboardShortcuts = value; }
#endif
        }

        public IList<KeyboardShortcut> KeyboardShortcuts
        {
            get { return _newKeyboardShortcuts; }
#if UNITY_EDITOR
            set { _newKeyboardShortcuts = value.ToArray(); }
#endif
        }

        public bool KeyboardEscapeClose
        {
            get { return _keyboardEscapeClose; }
#if UNITY_EDITOR
            set { _keyboardEscapeClose = value; }
#endif
        }

        public bool EnableBackgroundTransparency
        {
            get { return _enableBackgroundTransparency; }
#if UNITY_EDITOR
            set { _enableBackgroundTransparency = value; }
#endif
        }

        public bool RequireCode
        {
            get { return _requireEntryCode; }
#if UNITY_EDITOR
            set { _requireEntryCode = value; }
#endif
        }

        public bool RequireEntryCodeEveryTime
        {
            get { return _requireEntryCodeEveryTime; }
#if UNITY_EDITOR
            set { _requireEntryCodeEveryTime = value; }
#endif
        }

        public IList<int> EntryCode
        {
            get { return new ReadOnlyCollection<int>(_entryCode); }
            set
            {
                if (value.Count != 4)
                {
                    throw new Exception("Entry code must be length 4");
                }
                if (value.Any(p => p > 9 || p < 0))
                {
                    throw new Exception("All digits in entry code must be >= 0 and <= 9");
                }

                _entryCode = value.ToArray();
            }
        }

        public bool UseDebugCamera
        {
            get { return _useDebugCamera; }
#if UNITY_EDITOR
            set { _useDebugCamera = value; }
#endif
        }

        public int DebugLayer
        {
            get { return _debugLayer; }
#if UNITY_EDITOR
            set { _debugLayer = value; }
#endif
        }

        public float DebugCameraDepth
        {
            get { return _debugCameraDepth; }
#if UNITY_EDITOR
            set { _debugCameraDepth = value; }
#endif
        }

        public bool CollapseDuplicateLogEntries
        {
            get { return _collapseDuplicateLogEntries; }
#if UNITY_EDITOR
            set { _collapseDuplicateLogEntries = value; }
#endif
        }

        public bool RichTextInConsole
        {
            get { return _richTextInConsole; }
#if UNITY_EDITOR
            set { _richTextInConsole = value; }
#endif
        }

        public string ApiKey
        {
            get { return _apiKey; }
#if UNITY_EDITOR
            set
            {
                _apiKey = value;
                EditorUtility.SetDirty(this);
            }
#endif
        }

        public bool EnableBugReporter
        {
            get { return _enableBugReporter; }
#if UNITY_EDITOR
            set { _enableBugReporter = value; }
#endif
        }

        public IList<DefaultTabs> DisabledTabs
        {
            get { return _disabledTabs; }
#if UNITY_EDITOR
            set { _disabledTabs = value.ToArray(); }
#endif
        }

        /// <summary>
        /// Position for the triple-tap button
        /// </summary>
        public PinAlignment TriggerPosition
        {
            get { return _triggerPosition; }
#if UNITY_EDITOR
            set
            {
                if (_profilerAlignment == value)
                {
                    _profilerAlignment = _triggerPosition;
                }
                if (_optionsAlignment == value)
                {
                    _optionsAlignment = _triggerPosition;
                }
                _triggerPosition = value;
            }
#endif
        }

        public PinAlignment ProfilerAlignment
        {
            get { return _profilerAlignment; }
#if UNITY_EDITOR
            set
            {
                if (_triggerPosition == value)
                {
                    _triggerPosition = _profilerAlignment;
                }
                if (_optionsAlignment == value)
                {
                    _optionsAlignment = _profilerAlignment;
                }
                _profilerAlignment = value;
            }
#endif
        }

        public PinAlignment OptionsAlignment
        {
            get { return _optionsAlignment; }
#if UNITY_EDITOR
            set
            {
                if (_triggerPosition == value)
                {
                    _triggerPosition = _optionsAlignment;
                }
                if (_profilerAlignment == value)
                {
                    _profilerAlignment = _optionsAlignment;
                }
                _optionsAlignment = value;
            }
#endif
        }

        public ConsoleAlignment ConsoleAlignment
        {
            get { return _consoleAlignment; }
            set { _consoleAlignment = value; }
        }

        #endregion

        #region Serialization

        [SerializeField] private bool _isEnabled = true;

        [SerializeField] private bool _autoLoad = true;

        [SerializeField] private DefaultTabs _defaultTab = DefaultTabs.SystemInformation;

        [SerializeField] private TriggerEnableModes _triggerEnableMode = TriggerEnableModes.Enabled;

        [SerializeField] private TriggerBehaviours _triggerBehaviour = TriggerBehaviours.TripleTap;

        [SerializeField] private bool _enableKeyboardShortcuts = true;

        // Legacy keyboard shortcuts, should be upgraded
        [SerializeField] private KeyboardShortcut[] _keyboardShortcuts;

        // New keyboard shortcut array, containing upgraded shortcuts
        [SerializeField] private KeyboardShortcut[] _newKeyboardShortcuts = GetDefaultKeyboardShortcuts();

        [SerializeField] private bool _keyboardModifierControl = true;

        [SerializeField] private bool _keyboardModifierAlt = false;

        [SerializeField] private bool _keyboardModifierShift = true;

        [SerializeField] private bool _keyboardEscapeClose = true;

        [SerializeField] private bool _enableBackgroundTransparency = true;

        [SerializeField] private bool _collapseDuplicateLogEntries = true;

        [SerializeField] private bool _richTextInConsole = true;

        [SerializeField] private bool _requireEntryCode;

        [SerializeField] private bool _requireEntryCodeEveryTime;

        [SerializeField] private int[] _entryCode = {0, 0, 0, 0};

        [SerializeField] private bool _useDebugCamera;

        [SerializeField] private int _debugLayer = 5;

        [SerializeField] [Range(-100, 100)] private float _debugCameraDepth = 100f;

        [SerializeField] private string _apiKey = "";

        [SerializeField] private bool _enableBugReporter;

        [SerializeField] private DefaultTabs[] _disabledTabs = {};

        [SerializeField] private PinAlignment _profilerAlignment = PinAlignment.BottomLeft;

        [SerializeField] private PinAlignment _optionsAlignment = PinAlignment.BottomRight;

        [SerializeField] private ConsoleAlignment _consoleAlignment = ConsoleAlignment.Top;

        [SerializeField] private PinAlignment _triggerPosition = PinAlignment.TopLeft;

        #endregion

        #region Saving/Loading

        private static Settings GetOrCreateInstance()
        {
            var instance = Resources.Load<Settings>("SRDebugger/" + ResourcesName);

            if (instance == null)
            {
                // Create instance
                instance = CreateInstance<Settings>();

#if UNITY_EDITOR

                // Get resources folder path
                var resourcesPath = GetResourcesPath();

                if (resourcesPath == null)
                {
                    Debug.LogError(
                        "[SRDebugger] Error finding Resources path. Please make sure SRDebugger folder is intact");
                    return instance;
                }

                Debug.Log("[SRDebugger] Creating settings asset at {0}/{1}".Fmt(resourcesPath, ResourcesName));

                // Create directory if it doesn't exist
                Directory.CreateDirectory(resourcesPath);

                // Save instance if in editor
                AssetDatabase.CreateAsset(instance, resourcesPath + "/" + ResourcesName + ".asset");

#endif
            }

            return instance;
        }

#if UNITY_EDITOR

        private static string GetResourcesPath()
        {
            try
            {
                return Internal.Editor.SRDebugEditorUtil.GetRootPath() + ResourcesPath;
            }
            catch
            {
                return null;
            }
        }

#endif

        #endregion
    }
}
