﻿using SRDebugger.UI.Other;
using SRF;
using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI
{
    public class MobileMenuController : SRMonoBehaviourEx
    {
        private Button _closeButton;

        [SerializeField] private float _maxMenuWidth = 185f;

        [SerializeField] private float _peekAmount = 45f;

        private float _targetXPosition;

        [RequiredField] public RectTransform Content;

        [RequiredField] public RectTransform Menu;

        [RequiredField] public Button OpenButton;

        [RequiredField] public SRTabController TabController;

        public float PeekAmount
        {
            get { return _peekAmount; }
        }

        public float MaxMenuWidth
        {
            get { return _maxMenuWidth; }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            var parent = Menu.parent as RectTransform;

            var layoutElement = Menu.GetComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;

            // Set up menu anchors so it stretches to full height and has the screen width
            Menu.pivot = new Vector2(1, 1);

            Menu.offsetMin = new Vector2(1f, 0f);
            Menu.offsetMax = new Vector2(1f, 1f);

            Menu.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                Mathf.Clamp(parent.rect.width - PeekAmount, 0, MaxMenuWidth));

            Menu.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parent.rect.height);

            Menu.anchoredPosition = new Vector2(0, 0);

            if (_closeButton == null)
            {
                CreateCloseButton();
            }

            OpenButton.gameObject.SetActive(true);

            TabController.ActiveTabChanged += TabControllerOnActiveTabChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            var layoutElement = Menu.GetComponent<LayoutElement>();
            layoutElement.ignoreLayout = false;

            // Reset content position in case it has been moved by opening the menu
            Content.anchoredPosition = new Vector2(0, 0);

            _closeButton.gameObject.SetActive(false);
            OpenButton.gameObject.SetActive(false);

            TabController.ActiveTabChanged -= TabControllerOnActiveTabChanged;
        }

        private void CreateCloseButton()
        {
            var go = Content.CreateChild("CloseButton");
            var rect = go.AddComponent<RectTransform>();

            go.AddComponent<LayoutElement>().ignoreLayout = true;

            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Content.rect.width);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Content.rect.height);

            go.AddComponent<Image>().color = new Color(0, 0, 0, 0);

            _closeButton = go.AddComponent<Button>();
            _closeButton.transition = Selectable.Transition.None;

            _closeButton.onClick.AddListener(CloseButtonClicked);

            _closeButton.gameObject.SetActive(false);
        }

        private void CloseButtonClicked()
        {
            Close();
        }

        protected override void Update()
        {
            base.Update();

            var from = Content.anchoredPosition.x;

            if (Mathf.Abs(_targetXPosition - from) < 2.5f)
            {
                Content.anchoredPosition = new Vector2(_targetXPosition, Content.anchoredPosition.y);
            }
            else
            {
                Content.anchoredPosition =
                    new Vector2(SRMath.SpringLerp(from, _targetXPosition, 15f, Time.unscaledDeltaTime),
                        Content.anchoredPosition.y);
            }
        }

        private void TabControllerOnActiveTabChanged(SRTabController srTabController, SRTab srTab)
        {
            Close();
        }

        [ContextMenu("Open")]
        public void Open()
        {
            _targetXPosition = Menu.rect.width;
            _closeButton.gameObject.SetActive(true);
        }

        [ContextMenu("Close")]
        public void Close()
        {
            _targetXPosition = 0;
            _closeButton.gameObject.SetActive(false);
        }
    }
}
