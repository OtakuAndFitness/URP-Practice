﻿using System;
using SRDebugger.Internal;
using SRDebugger.Services;
using SRF;
using SRF.UI.Layout;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 169
#pragma warning disable 649

namespace SRDebugger.UI.Controls
{
    public class ConsoleLogControl : SRMonoBehaviourEx
    {
        [RequiredField] [SerializeField] private VirtualVerticalLayoutGroup _consoleScrollLayoutGroup;

        [RequiredField] [SerializeField] private ScrollRect _consoleScrollRect;

        private bool _isDirty;
        private bool _showErrors = true;
        private bool _showInfo = true;
        private bool _showWarnings = true;
        public Action<ConsoleEntry> SelectedItemChanged;
        private Vector2? _scrollPosition;

        public bool ShowErrors
        {
            get { return _showErrors; }
            set
            {
                _showErrors = value;
                SetIsDirty();
            }
        }

        public bool ShowWarnings
        {
            get { return _showWarnings; }
            set
            {
                _showWarnings = value;
                SetIsDirty();
            }
        }

        public bool ShowInfo
        {
            get { return _showInfo; }
            set
            {
                _showInfo = value;
                SetIsDirty();
            }
        }

        public bool EnableSelection
        {
            get { return _consoleScrollLayoutGroup.EnableSelection; }
            set { _consoleScrollLayoutGroup.EnableSelection = value; }
        }

        protected override void Awake()
        {
            base.Awake();

            _consoleScrollLayoutGroup.SelectedItemChanged.AddListener(OnSelectedItemChanged);
            Service.Console.Updated += ConsoleOnUpdated;
        }

        protected override void Start()
        {
            base.Start();
            SetIsDirty();
        }

        protected override void OnDestroy()
        {
            if (Service.Console != null)
            {
                Service.Console.Updated -= ConsoleOnUpdated;
            }

            base.OnDestroy();
        }

        private void OnSelectedItemChanged(object arg0)
        {
            var entry = arg0 as ConsoleEntry;

            if (SelectedItemChanged != null)
            {
                SelectedItemChanged(entry);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (_scrollPosition.HasValue)
            {
                _consoleScrollRect.normalizedPosition = _scrollPosition.Value;
                _scrollPosition = null;
            }

            if (_isDirty)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            if (_consoleScrollRect.normalizedPosition.y.ApproxZero())
            {
                _scrollPosition = _consoleScrollRect.normalizedPosition;
            }

            _consoleScrollLayoutGroup.ClearItems();

            var entries = Service.Console.Entries;

            for (var i = 0; i < entries.Count; i++)
            {
                var e = entries[i];

                if ((e.LogType == LogType.Error || e.LogType == LogType.Exception || e.LogType == LogType.Assert) &&
                    !ShowErrors)
                {
                    continue;
                }

                if (e.LogType == LogType.Warning && !ShowWarnings)
                {
                    continue;
                }

                if (e.LogType == LogType.Log && !ShowInfo)
                {
                    continue;
                }

                _consoleScrollLayoutGroup.AddItem(e);
            }

            _isDirty = false;
        }

        private void SetIsDirty()
        {
            _isDirty = true;
        }

        private void ConsoleOnUpdated(IConsoleService console)
        {
            SetIsDirty();
        }
    }
}
